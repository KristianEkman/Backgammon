using Backend.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Dto
{
    public static class Mapper
    {
        public static GameDto ToDto(this Game game)
        {
            var gameDto = new GameDto
            {
                BlackPlayer = game.BlackPlayer.ToDto(),
                WhitePlayer = game.WhitePlayer.ToDto(),
                CurrentPlayer = (PlayerColor)game.CurrentPlayer,
                PlayState = (GameState)game.PlayState,
                Points = game.Points.Select(p => p.ToDto()).ToArray(),
                Roll = game.Roll.Select(r => r.ToDto()).ToArray(),
                ValidMoves = game.ValidMoves.Select(m => m.ToDto()).ToArray(),                
            };
            return gameDto;
        }

        public static PlayerDto ToDto(this Player player)
        {
            var playerDto = new PlayerDto
            {
                PlayerColor = (PlayerColor)player.PlayerColor
            };
            return playerDto;
        }

        public static PointDto ToDto(this Point point)
        {
            var pointDto = new PointDto
            {
                BlackNumber = point.BlackNumber,
                WhiteNumber = point.WhiteNumber,
                Checkers = point.Checkers.Select(c => c.ToDto()).ToArray()                
            };
            return pointDto;
        }

        public static CheckerDto ToDto(this Checker checker)
        {
            var checkerDto = new CheckerDto
            {
                Color = (PlayerColor)checker.Color
            };
            return checkerDto;
        }

        public static DiceDto ToDto(this Dice dice)
        {
            var diceDto = new DiceDto
            {
                Used = dice.Used,
                Value = dice.Value
            };
            return diceDto;
        }

        public static MoveDto ToDto(this Move move)
        {
            var moveDto = new MoveDto
            {
                Color = (PlayerColor)move.Color,
                From = move.From.GetNumber(move.Color),
                To = move.To.GetNumber(move.Color),
                // recursing up in move tree
                NextMoves = move.NextMoves.Select(move => move.ToDto()).ToArray()
            };
            return moveDto;
        }
    }
}
