using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Dto.Actions
{
    public class AbortGameActionDto : ActionDto
    {
        public AbortGameActionDto()
        {
            this.actionName = ActionNames.abortGame;
        }
        public string gameId { get; set; }

    }
}
