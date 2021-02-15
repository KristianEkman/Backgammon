using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Dto.Actions
{
    public class GameEndedActionDto : ActionDto
    {
        public GameEndedActionDto()
        {
            this.actionName = ActionNames.gameEnded;
        }

        public GameDto game { get; set; }

    }
}

