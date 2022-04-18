using Microsoft.AspNetCore.Identity;

namespace Domain
{
    public class Diffie
    {
        public int Id { get; set; }
        public ulong PrivateKeyA { get; set; }
        public ulong PrivateKeyB { get; set; }
        public ulong ModulusP { get; set; }
        public ulong BaseG { get; set; }
        public ulong PublicKey { get; set; }
        public ulong SharedKey { get; set; }
        
        // add foreign key
        public string UserId { get; set; }
        public IdentityUser User { get; set; }
    }
}