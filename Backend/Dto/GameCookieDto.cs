using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Backend.Dto
{
    /// <summary>
    /// Stored as a cookie on the client to enable reconnects.
    /// </summary>
    public class GameCookieDto
    {
        public string id { get; set; }
        public PlayerColor color { get; set; }

        internal static GameCookieDto TryParse(string v)
        {
            try
            {
                return (GameCookieDto)JsonSerializer.Deserialize(v, typeof(GameCookieDto));
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
