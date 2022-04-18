using Microsoft.AspNetCore.Identity;

namespace Domain
{
    public class RSA
    {
        // E and N are the public key
        // D and M are the private key
        
        public int Id { get; set; }
        
        public ulong P { get; set; }
        public ulong Q { get; set; }
        public ulong N { get; set; }
        public ulong E { get; set; }
        public ulong M { get; set; }
        public ulong D { get; set; }
        public string Plaintext { get; set; }
        public string Ciphertext { get; set; }
        
        // add foreign key
        public string UserId { get; set; }
        public IdentityUser User { get; set; }
    }
}