namespace Kursach.Models.Statistics
{
    public record PositionStatistics
    {
        public double Ticks { get; init; }
        
        public string PetriObjectId { get; init; }
        
        public string PositionId { get; init; }
        
        public int Markers { get; init; }
    }
}