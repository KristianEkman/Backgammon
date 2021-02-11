using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Dto.Actions
{
    public class MovesMadeActionDto  : ActionDto
    {
        public MovesMadeActionDto()
        {
            this.actionName = ActionNames.movesMade;
        }

        public MoveDto move1 { get; set; }

        public MoveDto move2 { get; set; }

    }
}
