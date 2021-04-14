using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Dto.admin
{
    public class SummaryDto
    {
        public int reggedUsersToday;
        public int reggedUsers { get; set; }
        public int ongoingGames { get; set; }
        public int playedGamesTotal { get; set; }
        public int playedGamesToday { get; set; }
    }
}
