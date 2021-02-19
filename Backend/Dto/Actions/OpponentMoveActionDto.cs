
namespace Backend.Dto.Actions
{
    /// <summary>
    /// This action is used to show an animation on moves that the opponent does and undos.
    /// </summary>
    public class OpponentMoveActionDto : ActionDto
    {
        public OpponentMoveActionDto()
        {
            this.actionName = ActionNames.opponentMove;
        }
        public MoveDto move { get; set; }
    }
}
