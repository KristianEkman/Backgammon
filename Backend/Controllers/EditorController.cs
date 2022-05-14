using Backend.Dto;
using Backend.Dto.editor;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Text;

namespace Backend.Controllers
{
    [ApiController]
    public class EditorController : Controller
    {
        [Route("api/editor/gamestring")]
        [HttpPost]
        public GameStringResponseDto GameString(GameStringRequest g)
        {
			var value = GetGameString(g.game, g.dice);
            return new GameStringResponseDto { value = value};
        }


		private string GetGameString(GameDto g, DiceDto[] dice)
		{
			var s = new StringBuilder();

			var blackBar = g.points[0].checkers.Count(c => c.color == PlayerColor.black);
			s.Append($"b{blackBar}");
			s.Append(' ');

			for (int i = 1; i < 24; i++)
			{
				var checkers = g.points[i].checkers;
				if (checkers.Length > 0)
				{
					var color = checkers[0].color;
					if (color == PlayerColor.black)
						s.Append('b');
					else
						s.Append('w');
				}												
				s.Append(checkers.Length);
				s.Append(' ');
			}
			var whiteBar = g.points[25].checkers.Count(c => c.color == PlayerColor.white);
			s.Append($"w{whiteBar}");
			s.Append(' ');
			var blackHome = g.points[25].checkers.Count(c => c.color == PlayerColor.black);
			s.Append($"b{blackHome}"); s.Append(' ');
			var whiteHome = g.points[0].checkers.Count(c => c.color == PlayerColor.white);
			s.Append($"w{whiteHome}");			
			s.Append(' ');			
			s.Append(g.currentPlayer == PlayerColor.black ? 'b' : 'w');
			s.Append(' ');
			s.Append(dice[0].value);
			s.Append(' ');
			s.Append(dice[1].value);
			return s.ToString();
		}
	}
}
