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

    }
}
