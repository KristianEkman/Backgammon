using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Dto.Actions
{
    public class ConnectionInfoActionDto : ActionDto
    {
        public ConnectionInfoActionDto()
        {
            this.actionName = ActionNames.connectionInfo;
        }

        public ConnectionDto connection { get; set; }

    }
}
