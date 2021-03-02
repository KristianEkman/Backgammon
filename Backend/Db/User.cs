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

        public ICollection<Player> Players { get; set; } = new List<Player>();
    }
}
