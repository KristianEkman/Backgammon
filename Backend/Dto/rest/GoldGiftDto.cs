using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Dto.rest
{
    public class GoldGiftDto
    {
        public const int Gift = 200;
        public int gold { get; set; }
        public int lastFreeGold { get; set; }
    }
}