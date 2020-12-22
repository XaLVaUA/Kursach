using System;

namespace Kursach.Models.Statistics
{
    public record ResultStatistics
    {
        public double IdleProbability { get; init; }
        
        public double Queue1AverageLength { get; init; }
        
        public double Queue1MaxLength { get; init; }

        public double Queue2AverageLength { get; init; }

        public double Queue2MaxLength { get; init; }

        public void Print()
        {
            Console.WriteLine($"Idle probability: {IdleProbability}");
            Console.WriteLine($"Queue1 average length: {Queue1AverageLength}");
            Console.WriteLine($"Queue1 max length: {Queue1MaxLength}");
            Console.WriteLine($"Queue2 average length: {Queue2AverageLength}");
            Console.WriteLine($"Queue2 max length: {Queue2MaxLength}");
        }
    }
}