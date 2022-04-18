using System;
using System.Collections.Generic;
using System.Numerics;
using System.Security.Authentication.ExtendedProtection;

namespace RSA
{
    class Program
    {
        static void Main(string[] args)
        {  
            
            RSA rsa = new RSA();
            
            rsa.PrintAllValues();
            
            ulong plaintext = 12456;
            Console.WriteLine($"Plaintext: {plaintext}");

            ulong ciphertext = rsa.Encrypt(plaintext);
            Console.WriteLine($"Ciphertext: {ciphertext}");

            var decrypted = rsa.Decrypt(ciphertext);
            Console.WriteLine($"Plaintext (decrypted): {decrypted}");

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
        
        static void BreakRSA(ulong n, ulong ciphertext)
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
    }

    class RSA
    {
        private ulong p, q, n, m, d, e;

        public RSA()
        {
            Console.WriteLine("RSA");

            this.GetPrimes();

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

        public void PrintAllValues()
        {
            Console.WriteLine($"p: {this.p} q: {this.q}");
            Console.WriteLine($"n = p * q is {this.n}");
            Console.WriteLine($"m = (p - 1) * (q - 1) is {m}");
            Console.WriteLine($"d is {this.d}");
            Console.WriteLine($"e is {this.e}");
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

        public void GetPrimes()
        {
            ulong temp, qmax;
            
            Console.WriteLine("Enter prime p:");
            
            while (true)
            {
                try
                {
                    temp = Convert.ToUInt64(Console.ReadLine());

                    if (this.IsPrime(temp))
                    {
                        if ((ulong.MaxValue / temp) > 2)
                        {
                            this.p = temp;
                            qmax = ulong.MaxValue / temp;
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Error: value for p is too large, will cause overflow for n! \nPlease enter the number again:");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Error: p must be a prime number! \nPlease enter the number again:");
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"Error: {exception} \nPlease enter the number again:");
                }
            }
            
            //
            
            Console.WriteLine($"Enter prime q: (smaller than {qmax})");
            
            while (true)
            {
                try
                {
                    temp = Convert.ToUInt64(Console.ReadLine());

                    if (this.IsPrime(temp))
                    {

                        if (temp < qmax)
                        {
                            this.q = temp;
                            break;
                        }
                        else
                        {
                            Console.WriteLine($"Error: q must be smaller than {qmax} so ulong doesn't overflow! \nPlease enter the number again:");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Error: q must be a prime number! \nPlease enter the number again:");
                    }
                    
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"Error: {exception} \nPlease enter the number again:");
                }
            }
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
                    //return (baseVal * val) % mod;
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

        public ulong Decrypt(ulong ciphertext)
        {
            ulong plaintext = this.UlongPowMod(ciphertext, this.d, this.n);
            return plaintext;
        }
    }
}