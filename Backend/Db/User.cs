using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Db
{
    public class User
    {
        public const string AiUser = "ECC9A1FC-3E5C-45E6-BCE3-7C24DFE82C98";
        public Guid Id { get; set; }
        public string ProviderId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhotoUrl { get; set; }
        public bool ShowPhoto { get; set; }
        public string SocialProvider { get; set; }
        public int Elo { get; set; }
        public int GameCount { get; set; }
        public bool Admin { get; set; }
        public DateTime? Registered { get; set; }
        public string PreferredLanguage { get; set; }
        public string Theme { get; set; }
        public bool EmailNotifications { get; set; }
        public Guid EmailUnsubscribeId { get; set; }
        public int Gold { get; set; }
        public DateTime LastFreeGold { get; set; }
        public string LocalLogin { get; set; }
        public int PassHash { get; set; }

        public ICollection<Player> Players { get; set; } = new List<Player>();
        [InverseProperty("Receiver")]
        public ICollection<Message> ReceivedMessages { get; set; } = new List<Message>();
        [InverseProperty("Sender")]
        public ICollection<Message> SentMessages { get; set; } = new List<Message>();

    }
}
