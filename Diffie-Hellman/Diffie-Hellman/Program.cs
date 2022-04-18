using System;
using System.Collections.Generic;

namespace Diffie_Hellman
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("Diffie-Hellman");
            Console.WriteLine();

            // modulusP is a large prime number
            // baseG doesn't need to be very large or prime, it just has to be primitive root modulo modulusP
            // privateA and privateB can be any numbers from between 0 and modulusP
            
            Console.WriteLine("Input alice parameters:");
            DiffieObject alice = new DiffieObject();
            
            Console.WriteLine();
            
            Console.WriteLine("input bob parameters:");
            DiffieObject bob = new DiffieObject();
            
            Console.WriteLine();
            
            alice.MakeSharedKey(bob.PublicKey);
            bob.MakeSharedKey(alice.PublicKey);

            Console.WriteLine("Press enter to exit");
            Console.Read();
        }
    }

    class DiffieObject
    {
        private ulong _privateKey;
        public ulong ModulusP;
        public ulong BaseG;
        
        public ulong PublicKey;
        private ulong _sharedKey;

        public DiffieObject()
        {
            this.ModulusP = this.GetModulusP();
            this.BaseG = this.GetBaseG();
            this._privateKey = this.GetPrivateKey();

            this.PublicKey = MakePublicKey();
        }

        private ulong MakePublicKey()
        {
            ulong publicKey = UlongPowMod(this.BaseG, this._privateKey, this.ModulusP);

            return publicKey;
        }
        
        public void MakeSharedKey(ulong publicKeyElse)
        {
            this._sharedKey = UlongPowMod(publicKeyElse, this._privateKey, this.ModulusP);
            Console.WriteLine(this._sharedKey);
        }

        private ulong UlongPowMod(ulong a, ulong b, ulong mod)
        {
            if (b == 0) return 1 % mod;
            
            ulong poweredUp = a % mod;

            for (ulong i = 1; i < b; i++)
            {
                poweredUp = (poweredUp * a) % mod;
            }

            return poweredUp;
        }

        private bool IsPrime(ulong number)
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
        
        private bool IsPrimitiveRootModuloN(ulong baseG, ulong modulusP)
        {

            if (baseG > modulusP)
            {
                return false;
            }
                
            ulong[] allRemainders = new ulong[modulusP-1];
            List<ulong> remainders = new List<ulong>();
            
            
            for(ulong i=0; i<modulusP-1; i++)
            {
                allRemainders[i] = i+1;
            }
            

            for (ulong i = 1; i < modulusP; i++)
            {
                ulong tempRemainder = UlongPowMod(baseG, i, modulusP);
                    
                if (remainders.Contains(tempRemainder)) return false;
                    
                remainders.Add(tempRemainder);
            }
            
            
            foreach (ulong i in allRemainders)
            {
                if (!remainders.Contains(i)) return false;
            }

            return true;
        }

        private ulong GetModulusP()
        {
            ulong modulusP;

            Console.WriteLine("Input Public Parameter Modulus P:");
            
            while (true)
            {
                try
                {
                    modulusP = Convert.ToUInt64(Console.ReadLine());

                    if (this.IsPrime(modulusP))
                    {
                        break;
                    }
                    else
                    {
                        Console.WriteLine("ModulusP must be a prime number! Please enter the number again:");
                    }

                }
                catch(Exception e)
                {
                    Console.WriteLine($"Error: {e.Message} Please enter the number again:");
                }
            }
            
            return modulusP;
        }

        private ulong GetBaseG()
        {
            ulong baseG;
            
            Console.WriteLine("Input Public Parameter Base G:");

            while (true)
            {
                try
                {
                    baseG = Convert.ToUInt64(Console.ReadLine());

                    if (this.IsPrimitiveRootModuloN(baseG, this.ModulusP))
                    {
                        break;
                    }
                    else
                    {
                        Console.WriteLine("BaseG must be a primitive root modulo of ModulusP! Please enter the number again:");
                    }

                }
                catch(Exception e)
                {
                    Console.WriteLine($"Error: {e.Message} Please enter the number again:");
                }
            }

            return baseG;
        }

        private ulong GetPrivateKey()
        {
            ulong privateK;
            
            Console.WriteLine("Input Private Key:");
            
            while (true)
            {
                try
                {
                    privateK = Convert.ToUInt64(Console.ReadLine());

                    if (privateK < this.ModulusP)
                    {
                        break;
                    }
                    else
                    {
                        Console.WriteLine(
                            "The private key must be less than ModulusP! Please enter the number again:");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error: {e.Message} Please enter the number again:");
                }
            }

            return privateK;
        }
    }
}