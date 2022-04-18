using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch stopwatch = new Stopwatch();
            
            stopwatch.Start();
            
            RSA chuck = new RSA(1000099, 1000199);

            string ciphertext = chuck.EncryptString("JoJo was a man who thought he was a loner\n" +
                                                    "but he  knew it couldn't last\n\n" +
                                                    "Jojo left his home in Tucson, Arizona\n" +
                                                    "For some California grass\n\n" +
                                                    "Get back!...");
            
            Console.WriteLine("Ciphertext:\n" + ciphertext);
            
            Console.WriteLine();
            
            string plaintext = chuck.DecryptString(ciphertext);
            
            Console.WriteLine("Plaintext:\n" + plaintext);
            
            stopwatch.Stop();
            
            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts = stopwatch.Elapsed;

            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
            Console.WriteLine("RunTime " + elapsedTime);
        }
    }
    
   
    
    public class RSA
    {
        private ulong p, q, n, m, d, e;

        public RSA(ulong p, ulong q)
        {
            if (CheckPrimes(p, q))
            {
                this.p = p;
                this.q = q;
            }

            this.n = this.p * this.q;

            this.m = (p - 1) * (q - 1);
            
            for(this.e=2; e<ulong.MaxValue; e++)
            {
                if (this.GCD(this.m, this.e) == 1) break;
            }

            for (ulong k = 2; k < ulong.MaxValue; k++)
            {
                if ((1 + k * this.m) % this.e == 0)
                {
                    this.d = (1 + k * this.m) / this.e;
                    break;
                }
            }
        }
        
        public bool CheckPrimes(ulong p, ulong q)
        {
            ulong qmax=0;
            
            if (this.IsPrime(p))
            {
                if ((ulong.MaxValue / p) > 2)
                {
                    qmax = ulong.MaxValue / p;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
            
            if (this.IsPrime(q))
            {
                if (q < qmax)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public ulong GCD(ulong a, ulong b)
        {
            if (a == 0) return b;
            return GCD(b % a, a);
        }
        
        public bool IsPrime(ulong number)
        {
            if (number <= 1) return false;
            if (number == 2) return true;
            if (number % 2 == 0) return false;

            ulong boundary = (ulong)Math.Floor(Math.Sqrt(number));

            for (ulong i = 3; i <= boundary; i += 2)
            {
                if (number % i == 0) { return false; }
            }
            
            return true;
        }

        public ulong MulMod(ulong a, ulong b, ulong mod)
        {
            ulong res = 0; // Initialize result 
            a = a % mod; 
            while (b > 0) 
            { 
                // If b is odd, add 'a' to result 
                if (b % 2 == 1) 
                    res = (res + a) % mod; 
  
                // Multiply 'a' with 2 
                a = (a * 2) % mod; 
  
                // Divide b by 2 
                b /= 2; 
            } 
  
            // Return result 
            return res % mod; 
        }
        
        public ulong UlongPowMod(ulong val, ulong exp, ulong mod, ulong baseVal=0)
        {
            // this whole method uses this logic: 
            // 62^64 % 133 == (62^2)^32 % 133 == 3844^32 % 133 == 120^32 % 133 (3844 % 133 == 120)
            // this repeats until the exponent is equal to 1

            ulong newVal, newExp;
            
            if (exp == 0) return 1 % mod;

            if (exp == 1)
            {
                if (baseVal == 0)
                {
                    return val % mod;
                }
                else
                {
                    //return (baseVal * val) % mod;
                    return MulMod(baseVal, val, mod);
                }
            } 
            
            if (exp % 2 == 0)
            {
                //newVal = (val * val) % mod;
                newVal = MulMod(val, val, mod);
                newExp = exp/2;
            }
            else
            {
                //newVal = (val * val) % mod;
                newVal = MulMod(val, val, mod);
                newExp = (exp - 1) / 2;
                if (baseVal == 0)
                {
                    baseVal = val;
                }
                else
                {
                    //baseVal = (baseVal * val) % mod;
                    baseVal = MulMod(baseVal, val, mod);
                }
            }

            return UlongPowMod(newVal, newExp, mod, baseVal);
        }

        public ulong Encrypt(ulong plaintext)
        {
            ulong ciphertext = this.UlongPowMod(plaintext, this.e, this.n);
            return ciphertext;
        }

        /*
        public string EncryptString(string plaintext)
        {
            byte[] plaintextBytes = Encoding.Default.GetBytes(plaintext);

            List<ulong> ulongList = new List<ulong>();
            
            byte[] tempArr = {0,0,0,0,0,0,0,0};
            foreach (byte b in plaintextBytes)
            {
                tempArr[0] = b;
                ulongList.Add(BitConverter.ToUInt64(tempArr));
            }

            string ciphertext = "";
            foreach (ulong p in ulongList)
            {
                ulong ciphertextUlong = Encrypt(p);
                byte[] ciphertextBytes = BitConverter.GetBytes(ciphertextUlong);
                string ciphertextTemp = Convert.ToBase64String(ciphertextBytes);

                ciphertext += ciphertextTemp;
            }
            
            return ciphertext;
        }
        */
        
        public string EncryptString(string plaintext)
        {
            byte[] plaintextBytes = Encoding.Default.GetBytes(plaintext);

            List<ulong> ulongList = new List<ulong>();
            
            byte[] tempArr = {0,0,0,0,0,0,0,0};
            foreach (byte b in plaintextBytes)
            {
                tempArr[0] = b;
                ulongList.Add(BitConverter.ToUInt64(tempArr));
            }

            List<byte> byteList = new List<byte>();
            
            foreach (ulong u in ulongList)
            {
                ulong ciphertextUlong = Encrypt(u);
                byte[] ciphertextBytes = BitConverter.GetBytes(ciphertextUlong);

                foreach (byte b in ciphertextBytes)
                {
                    byteList.Add(b);
                }
            }
            
            byte[] byteArr = new byte[byteList.Count];

            for (int i = 0; i < byteList.Count; i++)
            {
                byteArr[i] = byteList[i];
            }

            string ciphertext = Convert.ToBase64String(byteArr);

            return ciphertext;
        }

        public ulong Decrypt(ulong ciphertext)
        {
            ulong plaintext = this.UlongPowMod(ciphertext, this.d, this.n);
            return plaintext;
        }

        /*
        public string DecryptString(string ciphertext)
        {
            // This takes base64 string
            int len = ciphertext.Length;
            
            List<string> stringList = new List<string>();

            string tempBase64 = "";
            for (int i = 1; i < len+1; i++)
            {
                if (i % 12 != 0)
                {
                    tempBase64 += ciphertext[i - 1];
                }
                else
                {
                    tempBase64 += ciphertext[i - 1];
                    stringList.Add(tempBase64);
                    tempBase64 = "";
                }
            }


            string plaintext = "";
            foreach (string s in stringList)
            {
                byte[] ciphertextBytes = Convert.FromBase64String(s);

                ulong tempUlongCipher = BitConverter.ToUInt64(ciphertextBytes);

                ulong tempUlongPlain = Decrypt(tempUlongCipher);
                
                byte[] tempBytes = BitConverter.GetBytes(tempUlongPlain);

                string tempString = Encoding.Default.GetString(tempBytes);

                plaintext += tempString[0];
            }

            return plaintext;
        }
        */
        
        public string DecryptString(string ciphertext)
        {
            // This takes base64 string
            
            byte[] ciphertextBytes = Convert.FromBase64String(ciphertext);
            
            List<ulong> ulongList = new List<ulong>();

            byte[] tempByteArr = new byte[8];
            for (int i = 0; i < ciphertextBytes.Length; i++)
            {
                tempByteArr[i%8] = ciphertextBytes[i];
                if ((i + 1) % 8 == 0)
                {
                    ulongList.Add(BitConverter.ToUInt64(tempByteArr));
                    tempByteArr = new byte[8];
                }
            }

            string plaintext = "";
            foreach (ulong u in ulongList)
            {
                ulong tempUlongPlain = Decrypt(u);
                
                byte[] tempBytes = BitConverter.GetBytes(tempUlongPlain);

                string tempString = Encoding.Default.GetString(tempBytes);

                plaintext += tempString[0];
            }

            return plaintext;
        }
    }
}