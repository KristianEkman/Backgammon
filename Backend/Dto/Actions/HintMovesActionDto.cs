using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Dto.Actions
{
    public class HintMovesActionDto : ActionDto
    {
        public HintMovesActionDto()
        {
            this.actionName = ActionNames.hintMoves;
        }

        public MoveDto[] moves{ get; set; }

    }
}
