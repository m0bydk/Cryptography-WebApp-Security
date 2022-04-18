using System;
using System.Collections.Generic;
using System.Text;

namespace Crypto
{
    public class RSA
    {
        public ulong p, q, n, m, d, e;

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
                    return MulMod(baseVal, val, mod);
                }
            } 
            
            if (exp % 2 == 0)
            {
                newVal = MulMod(val, val, mod);
                newExp = exp/2;
            }
            else
            {
                newVal = MulMod(val, val, mod);
                newExp = (exp - 1) / 2;
                if (baseVal == 0)
                {
                    baseVal = val;
                }
                else
                {
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

    public static class RSA_static
    {
        public static bool CheckPrimes(ulong p, ulong q)
        {
            ulong qmax=0;
            
            if (IsPrime(p))
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
            
            if (IsPrime(q))
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
        
        static ulong MulMod(ulong a, ulong b, ulong mod)
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
        
        static ulong UlongPowMod(ulong val, ulong exp, ulong mod, ulong baseVal=0)
        {

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
                    return MulMod(baseVal, val, mod);
                }
            } 
            
            if (exp % 2 == 0)
            {
                newVal = MulMod(val, val, mod);
                newExp = exp/2;
            }
            else
            {
                newVal = MulMod(val, val, mod);
                newExp = (exp - 1) / 2;
                if (baseVal == 0)
                {
                    baseVal = val;
                }
                else
                {
                    baseVal = MulMod(baseVal, val, mod);
                }
            }

            return UlongPowMod(newVal, newExp, mod, baseVal);
        }

        static bool IsPrime(ulong number)
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
        
        static ulong GCD(ulong a, ulong b)
        {
            if (a == 0) return b;
            return GCD(b % a, a);
        }
        
        public static void BreakRSA(ulong n, ulong ciphertext)
        {
            ulong p, q;
            
            // brute force - find p and q
            for (ulong i = 0; true; i++)
            {
                if (IsPrime(i))
                {
                    Console.WriteLine("Trying: " + i);
                    if (n % i == 0)
                    {
                        Console.WriteLine("Found: " + i);
                        p = i;
                        q = n / p;
                        break;
                    }
                }
            }
            
            ulong m, d=0, e;

            m = (p - 1) * (q - 1);
            
            // calculate e
            for(e=2; e<ulong.MaxValue; e++)
            {
                if (GCD(m, e) == 1) break;
            }
            
            // calculate d
            for (ulong k = 2; k < ulong.MaxValue; k++)
            {
                if ((1 + k * m) % e == 0)
                {
                    d = (1 + k * m) / e;
                    break;
                }
            }
            
            // decrypt
            ulong plaintext = UlongPowMod(ciphertext, d, n);

            Console.WriteLine("Plaintext is: " + plaintext);
        }
        
        public static ulong Decrypt(ulong d, ulong n, ulong ciphertext)
        {
            ulong plaintext = UlongPowMod(ciphertext, d, n);
            return plaintext;
        }
        
        public static string DecryptString(ulong d, ulong n, string ciphertext)
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
                ulong tempUlongPlain = Decrypt(d, n, u);
                
                byte[] tempBytes = BitConverter.GetBytes(tempUlongPlain);

                string tempString = Encoding.Default.GetString(tempBytes);

                plaintext += tempString[0];
            }

            return plaintext;
        }
    }
}