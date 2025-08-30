// See https://aka.ms/new-console-template for more information

using System.Security.Cryptography;
using RSAImplementation;

var keys = RSAMethods.CreateKeys(1000000000000037,
1000000000000091
);
Console.WriteLine($"Modulus is {keys.Item1}, Private Key Exponent is {keys.Item2}, Public Key Exponent is {keys.Item3}");

var message = "Hello World";
var numericMessage = RSAMethods.MessageToBigInteger(message);
var encryptedMessage = RSAMethods.Encrypt(numericMessage, keys.Item3, keys.Item1);
Console.WriteLine(RSAMethods.Decrypt(encryptedMessage, keys.Item2, keys.Item1));
