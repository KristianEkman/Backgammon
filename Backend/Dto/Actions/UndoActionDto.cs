using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Dto.Actions
{
    public class UndoActionDto : ActionDto
    {
        public UndoActionDto()
        {
            this.actionName = ActionNames.undoMove;
        }
    }
}
