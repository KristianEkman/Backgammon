
namespace Backend.Dto
{
    public class PointDto
    {
        public int blackNumber { get; set; }
        public CheckerDto[] checkers { get; set; }
        public int whiteNumber { get; set; }
    }
}