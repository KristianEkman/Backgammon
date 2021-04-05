using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Db
{
    public class User
    {
        public Guid Id { get; set; }
        public string ProviderId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhotoUrl { get; set; }
        public string SocialProvider { get; set; }
        public int Elo { get; set; }
        public int GameCount { get; set; }
        public bool Admin { get; set; }
        public DateTime? Registered { get; set; }
        public ICollection<Player> Players { get; set; } = new List<Player>();
        public const string AiUser = "ECC9A1FC-3E5C-45E6-BCE3-7C24DFE82C98";
    }
}
