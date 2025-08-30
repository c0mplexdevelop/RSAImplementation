using System.Numerics;

namespace RSAImplementation;

using System;

public static class RSAMethods
{
    public static BigInteger ModulusOfTwoPrimes(BigInteger firstPrime, BigInteger secondPrime)
    {
        return firstPrime * secondPrime;
    }

    private static BigInteger CalculateEulierTotient(BigInteger firstPrime, BigInteger secondPrime)
    {
        return (firstPrime - 1) * (secondPrime - 1);
    }

    private static BigInteger CalculateGcd(BigInteger a, BigInteger b)
    {
        Console.WriteLine($"The two numbers are {a} and {b}");
        // Lets use Euclid's Algorithm here
        while (b != 0) {
            Console.WriteLine($"a is {a} and b is {b}");
            (a, b) = (b, a % b);
            Console.WriteLine($"a is now {a} and b is now {b}");
        }


        return a;
    }

    private static BigInteger CalculatePublicEncryptionExponent(BigInteger phi)
    {
        for (var idx = 3; idx < phi; idx += 2)
        {
            var coprime = CalculateGcd(idx, phi);
            if (coprime == 1)
            {
                Console.WriteLine($"The coprime of phi {phi} is {idx}");
                return idx;
            }
        }

        return -1; // No found coprime
    }

    private static BigInteger CalculatePrivateEncryptionExponent(BigInteger e, BigInteger phi)
    {
        /*
         * Private Key Exponents are such that where d, the private key exponenet
         * when multiplied by the public exponent (e), is equal to 1 % phi(modulus)
         * In other words, d is the modular multiplicative inverse of e % phi(modulus)
         */

        // for (BigInteger idx = 2; idx < phi; idx++)
        // {
        //     Console.WriteLine($"Calculating the product between {idx} and {e}");
        //     var product = BigInteger.Multiply(idx, e);
        //     if (product % phi == 1)
        //     {
        //         Console.WriteLine($"{idx} is the multiplicative inverse");
        //         return product;
        //     }
        //
        //     Console.WriteLine($"{idx} is not the multiplicative inverse");
        // }
        //
        // return -1; // Should not happen
        BigInteger originalPhi = phi;
        
        // We are looking for the coefficient of 'e' in Bézout's identity.
        // Let's call the coefficients 's' and 't'.
        // prev_s is the coefficient for the previous remainder, s is for the current.
        BigInteger prev_s = 1;
        BigInteger s = 0;

        // The algorithm proceeds like the standard Euclidean algorithm
        while (phi != 0)
        {
            BigInteger quotient = e / phi;

            // Standard GCD step: update e and phi
            (e, phi) = (phi, e % phi);

            // Update the coefficients for the original inputs
            // The new 's' is calculated from the previous two 's' values.
            (prev_s, s) = (s, prev_s - quotient * s);
        }

        // At the end of the loop, 'e' holds the GCD.
        BigInteger gcd = e;

        // An inverse exists only if the GCD is 1.
        if (gcd != 1)
        {
            throw new ArgumentException("Inverse does not exist because 'e' and 'phi' are not coprime.");
        }

        // The final coefficient 'prev_s' is our modular inverse.
        // It might be negative, so we adjust it to be in the range [1, phi-1].
        BigInteger inverse = prev_s;
        
        // This is a robust way to handle negative results: (inverse % m + m) % m
        return (inverse % originalPhi + originalPhi) % originalPhi;
        
    }

    public static Tuple<BigInteger, BigInteger, BigInteger> CreateKeys(BigInteger firstPrime, BigInteger secondPrime)
    {
        var modulus = firstPrime * secondPrime;
        var phi = CalculateEulierTotient(firstPrime, secondPrime);
        var publicKeyEncryptionExponent = CalculatePublicEncryptionExponent(phi);
        var privateKeyEncryptionExponent = CalculatePrivateEncryptionExponent(publicKeyEncryptionExponent, phi);
        
        return new Tuple<BigInteger, BigInteger, BigInteger>(modulus, privateKeyEncryptionExponent, publicKeyEncryptionExponent);
    }

    public static BigInteger MessageToBigInteger(string str)
    {
        byte[] stringByteArray = System.Text.Encoding.UTF8.GetBytes(str);
        return new BigInteger(stringByteArray, isUnsigned: true, isBigEndian: true);
    }

    private static string BigIntegerToMessage(BigInteger cipherText)
    {
        byte[] cipherTextByteArray = cipherText.ToByteArray(isUnsigned: true, isBigEndian: true);
        return System.Text.Encoding.UTF8.GetString(cipherTextByteArray);
    }

    public static BigInteger Encrypt(BigInteger message, BigInteger publicKey, BigInteger modulus)
    {
        BigInteger encryptedMessage = BigInteger.ModPow(message, publicKey, modulus);
        return encryptedMessage;
    }

    public static string Decrypt(BigInteger cipherText, BigInteger privateKey, BigInteger modulus)
    {
        var decryptedMessage = BigInteger.ModPow(cipherText, privateKey, modulus);
        var stringMessage = BigIntegerToMessage(decryptedMessage);
        return stringMessage;
    }
}