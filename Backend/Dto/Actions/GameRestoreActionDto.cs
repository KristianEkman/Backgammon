using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Dto.Actions
{
    public class GameRestoreActionDto : ActionDto
    {
        public GameRestoreActionDto()
        {
            actionName = ActionNames.gameRestore;
        }
        public GameDto game { get; set; }
        public PlayerColor color { get; set; }
        public DiceDto[] dices { get; set; }
    }
}
