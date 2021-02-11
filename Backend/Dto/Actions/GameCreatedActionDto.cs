using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Dto
{
    public class GameCreatedActionDto : ActionDto
    {
        public GameCreatedActionDto()
        {
            this.actionName = ActionNames.gameCreated;
        }
        
        public GameDto game { get; set; }
    }
}
