namespace Backend.Dto.editor
{
    public class GameStringRequest
    {
        public GameDto game { get; set; }

        public DiceDto[] dice { get;set;}

    }
}
