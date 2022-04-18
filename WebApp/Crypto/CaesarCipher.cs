using System;
using System.Text;

namespace Crypto
{
    public static class CaesarCipher
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Caesar");
            
            Console.WriteLine("Input string:");
            string plaintext = Console.ReadLine();
            
            Console.WriteLine("Input shift amount:");
            
            if (Int32.TryParse(Console.ReadLine(), out var shiftAmount))
            {
                shiftAmount = shiftAmount % 255;
                if (shiftAmount == 0) { Console.WriteLine("Shift amount cannot be multiples of 255 or 0"); }
                else { Console.WriteLine($"Caesar key is {shiftAmount}"); }
            }
            else
            {
                Console.WriteLine("Input must be an integer!");
                return;
            }

            string ciphertext = EncryptString(plaintext, shiftAmount, Encoding.Default);
            Console.WriteLine("ciphertext: "+ciphertext);

            string decrypted = DecryptString(ciphertext, shiftAmount, Encoding.Default);
            Console.WriteLine("decrypted plaintext: "+decrypted);
            
            Console.WriteLine("Press enter to exit...");
            Console.ReadLine();
        }

        public static byte[] Encrypt(byte[] plaintext, byte shiftAmount)
        {
            var ciphertext = new byte[plaintext.Length];
            
            for (int i = 0; i < plaintext.Length; i++)
            {
                var newCharValue = (plaintext[i] + shiftAmount) % byte.MaxValue;
                
                if (newCharValue == 0) { newCharValue = byte.MaxValue; }
            
                ciphertext[i] = (byte)newCharValue;
            }

            return ciphertext;
        }
        
        public static string EncryptString(string plaintext, int shiftAmount, Encoding encoding)
        {
            var plaintextBytes = encoding.GetBytes(plaintext);
            byte key = (byte) shiftAmount;

            var ciphertextBytes = Encrypt(plaintextBytes, key);

            string ciphertext = Convert.ToBase64String(ciphertextBytes);

            return ciphertext;
        }
         
        public static byte[] Decrypt(byte[] ciphertext, byte shiftAmount)
        {
            var plaintext = new byte[ciphertext.Length];

            for (int i = 0; i < ciphertext.Length; i++)
            {
                var newCharValue = (ciphertext[i] - shiftAmount) % byte.MaxValue;
                
                if (newCharValue == 0){ newCharValue = byte.MaxValue; }
                
                plaintext[i] = (byte) newCharValue;
            }

            return plaintext;
        }

        public static string DecryptString(string ciphertext, int shiftAmount, Encoding encoding)
        {
            var ciphertextBytes = Convert.FromBase64String(ciphertext);
            byte key = (byte) shiftAmount;

            var plaintextBytes = Decrypt(ciphertextBytes, key);
            
            string plaintext = encoding.GetString(plaintextBytes);

            return plaintext;
        }
        
    }
}