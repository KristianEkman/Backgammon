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
                id = game.Id.ToString(),
                blackPlayer = game.BlackPlayer.ToDto(),
                whitePlayer = game.WhitePlayer.ToDto(),
                currentPlayer = (PlayerColor)game.CurrentPlayer,
                playState = (GameState)game.PlayState,
                points = game.Points.Select(p => p.ToDto()).ToArray(),
                validMoves = game.ValidMoves.Select(m => m.ToDto()).ToArray(),                
            };
            return gameDto;
        }

        public static PlayerDto ToDto(this Player player)
        {
            var playerDto = new PlayerDto
            {
                playerColor = (PlayerColor)player.PlayerColor
            };
            return playerDto;
        }

        public static PointDto ToDto(this Point point)
        {
            var pointDto = new PointDto
            {
                blackNumber = point.BlackNumber,
                whiteNumber = point.WhiteNumber,
                checkers = point.Checkers.Select(c => c.ToDto()).ToArray()                
            };
            return pointDto;
        }

        public static CheckerDto ToDto(this Checker checker)
        {
            var checkerDto = new CheckerDto
            {
                color = (PlayerColor)checker.Color
            };
            return checkerDto;
        }

        public static DiceDto ToDto(this Dice dice)
        {
            var diceDto = new DiceDto
            {
                used = dice.Used,
                value = dice.Value
            };
            return diceDto;
        }

        public static MoveDto ToDto(this Move move)
        {
            var moveDto = new MoveDto
            {
                color = (PlayerColor)move.Color,
                from = move.From.GetNumber(move.Color),
                to = move.To.GetNumber(move.Color),
                // recursing up in move tree
                nextMoves = move.NextMoves.Select(move => move.ToDto()).ToArray()
            };
            return moveDto;
        }
    }
}
