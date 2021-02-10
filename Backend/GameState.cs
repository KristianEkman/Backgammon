using Backend.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace Backend
{
    public class GameState
    {
        public GameState(WebSocket client1)
        {
            Game = new Game();
            Client1 = client1;
        }

        public WebSocket Client1 { get; set; }
        public WebSocket Client2 { get; set; }
        public Game Game { get; set; }

        internal void StartGame()
        {
           
        }
    }
}
