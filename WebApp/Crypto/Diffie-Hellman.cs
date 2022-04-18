using System;
using System.Collections.Generic;

namespace Crypto
{
    public class Diffie_Hellman
    {
        
        private ulong _privateKey;
        public ulong ModulusP;
        public ulong BaseG;
        
        public ulong PublicKey;
        public ulong SharedKey;

        public Diffie_Hellman(ulong modulusP, ulong baseG, ulong privateKey)
        {
            if (this.IsModulusP(modulusP))
            {
                this.ModulusP = modulusP;
            }

            if (this.IsBaseG(baseG))
            {
                this.BaseG = baseG;
            }

            if (this.IsPrivateKey(privateKey))
            {
                this._privateKey = privateKey;
            }
            
            this.PublicKey = MakePublicKey();
        }

        private ulong MakePublicKey()
        {
            ulong publicKey = UlongPowMod(this.BaseG, this._privateKey, this.ModulusP);

            return publicKey;
        }
        
        public void MakeSharedKey(ulong publicKeyElse)
        {
            this.SharedKey = UlongPowMod(publicKeyElse, this._privateKey, this.ModulusP);
            Console.WriteLine(this.SharedKey);
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

        public bool IsModulusP(ulong modulusP)
        {

            if (this.IsPrime(modulusP))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsBaseG(ulong baseG)
        {
            if (this.IsPrimitiveRootModuloN(baseG, this.ModulusP))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsPrivateKey(ulong privateK)
        {
            if (privateK < this.ModulusP)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }

    public static class Diffie_Hellman_Static
    {
        public static ulong UlongPowMod(ulong a, ulong b, ulong mod)
        {
            if (b == 0) return 1 % mod;
            
            ulong poweredUp = a % mod;

            for (ulong i = 1; i < b; i++)
            {
                poweredUp = (poweredUp * a) % mod;
            }

            return poweredUp;
        }
        static bool IsPrimitiveRootModuloN(ulong baseG, ulong modulusP)
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
        public static bool IsPrime(ulong number)
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
        
        public static bool IsModulusP(ulong modulusP)
        {

            if (IsPrime(modulusP))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool IsBaseG(ulong baseG, ulong ModulusP)
        {
            if (IsPrimitiveRootModuloN(baseG, ModulusP))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool IsPrivateKey(ulong privateK, ulong ModulusP)
        {
            if (privateK < ModulusP)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool IsInputOK(ulong modulusP, ulong baseG, ulong privateA, ulong privateB)
        {

            if (!IsModulusP(modulusP))
            {
                return false;
            }

            if (!IsBaseG(baseG, modulusP))
            {
                return false;
            }

            if (!IsPrivateKey(privateA, modulusP))
            {
                return false;
            }
            
            if (!IsPrivateKey(privateB, modulusP))
            {
                return false;
            }

            return true;

        }
    }
}