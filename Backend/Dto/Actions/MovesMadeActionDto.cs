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

        public MoveDto[] moves { get; set; }


    }
}
