using System;
using Microsoft.AspNetCore.Identity;

namespace Domain
{
    public class Caesar
    {
        public int Id { get; set; }
        public int Key { get; set; }
        public string Plaintext { get; set; }
        public string Ciphertext { get; set; }
        
        // add foreign key
        public string UserId { get; set; }
        public IdentityUser User { get; set; }
    }
}