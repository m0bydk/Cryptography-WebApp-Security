using System;
using System.Text;

namespace Vigenere_Cipher
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Vigenere");
            Console.WriteLine("Input string:");
            string plaintext = Console.ReadLine();
            
            Console.WriteLine("Input key:");
            string key = Console.ReadLine();

            string ciphertext = EncryptString(plaintext, key, Encoding.Default);
            Console.WriteLine(ciphertext);

            string decrypted = DecryptString(ciphertext, key, Encoding.Default);
            Console.WriteLine(decrypted);

            Console.WriteLine("Press enter to exit");
            Console.ReadLine();
        }

        static byte[] Encrypt(byte[] plaintext, byte[] key)
        {

            var ciphertext = new byte[plaintext.Length];
            var adjustedKey = AdjustKey(key, plaintext.Length);

            for (int i = 0; i < plaintext.Length; i++)
            {

                var newCharValue = (plaintext[i] + adjustedKey[i]) % byte.MaxValue;
                if (newCharValue == 0) { newCharValue = byte.MaxValue; }

                ciphertext[i] = (byte) newCharValue;
            }

            return ciphertext;
        }

        static string EncryptString(string plaintext, string key, Encoding encoding)
        {
            var plaintextBytes = encoding.GetBytes(plaintext);
            var keyBytes = encoding.GetBytes(key);

            var ciphertextBytes = Encrypt(plaintextBytes, keyBytes);

            string ciphertext = Convert.ToBase64String(ciphertextBytes);

            return ciphertext;
        }

        static byte[] Decrypt(byte[] ciphertext, byte[] key)
        {
            var plaintext = new byte[ciphertext.Length];
            var adjustedKey = AdjustKey(key, ciphertext.Length);

            for (int i = 0; i < ciphertext.Length; i++)
            {
                var newCharValue = (ciphertext[i] - adjustedKey[i]) % byte.MaxValue;
                if (newCharValue == 0) { newCharValue = byte.MaxValue; }

                plaintext[i] = (byte) newCharValue;
            }

            return plaintext;
        }

        static string DecryptString(string ciphertext, string key, Encoding encoding)
        {
            var ciphertextBytes = Convert.FromBase64String(ciphertext);
            var keyBytes = encoding.GetBytes(key);

            var plaintextBytes = Decrypt(ciphertextBytes, keyBytes);

            string plaintext = encoding.GetString(plaintextBytes);

            return plaintext;
        }
        
        static byte[] AdjustKey(byte[] key, int len)
        {
            var newKey = new byte[len];

            for (int i = 0; i < len; i++)
            {
                newKey[i] = key[i % key.Length];
            }

            return newKey;
        }
    }
}