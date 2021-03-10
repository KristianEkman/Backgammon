using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Dto
{
    public class DicesRolledActionDto : ActionDto
    {
        
        public DicesRolledActionDto()
        {
            this.actionName = ActionNames.dicesRolled;
        }        

        public DiceDto[] dices { get; set; }
        public PlayerColor playerToMove { get; set; }
        public MoveDto[] validMoves { get; set; }
        public int moveTimer { get; set; }

        // todo: maybe rewrite to have a relation between dice and move
    }
}
