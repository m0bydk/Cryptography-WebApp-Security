using System;
using System.Text;

// namespace is the same as in c++; we can refer to functions using their namespace so different ones sharing the same name don't confuse the program.
namespace Caesar_Cipher
{
    // main class
    class Program
    {
        // void is just the return value of the method;
        // static methods belong to the class itself so you don't need to create an object to use that method;
        // for example Console.WriteLine(Math.Sqrt(144)); Math is the class we're using the method Sqrt() from;
        // if you create a static class, that means you can't create an object from it and it will basically act as a container for methods.
        
        // Encrypt() method takes byte array to encrypt, byte shiftAmount and returns byte array ciphertext, same with Decrypt().
        // EncryptString() method takes string to encrypt, int shiftAmount and returns a base64 string ciphertext.
        // DecryptString() takes base64 string and returns in in the encoding you provide.
        
        
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

        static byte[] Encrypt(byte[] plaintext, byte shiftAmount)
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
        
        static string EncryptString(string plaintext, int shiftAmount, Encoding encoding)
        {
            var plaintextBytes = encoding.GetBytes(plaintext);
            byte key = (byte) shiftAmount;

            var ciphertextBytes = Encrypt(plaintextBytes, key);

            string ciphertext = Convert.ToBase64String(ciphertextBytes);

            return ciphertext;
        }
         
        static byte[] Decrypt(byte[] ciphertext, byte shiftAmount)
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

        static string DecryptString(string ciphertext, int shiftAmount, Encoding encoding)
        {
            var ciphertextBytes = Convert.FromBase64String(ciphertext);
            byte key = (byte) shiftAmount;

            var plaintextBytes = Decrypt(ciphertextBytes, key);
            
            string plaintext = encoding.GetString(plaintextBytes);

            return plaintext;
        }
        
    }
}
