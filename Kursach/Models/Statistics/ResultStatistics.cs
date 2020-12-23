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
            Console.WriteLine($"Idle probability: {Math.Round(IdleProbability, 8)}");
            Console.WriteLine($"Queue1 average length: {Math.Round(Queue1AverageLength, 8)}");
            Console.WriteLine($"Queue1 max length: {Math.Round(Queue1MaxLength, 8)}");
            Console.WriteLine($"Queue2 average length: {Math.Round(Queue2AverageLength, 8)}");
            Console.WriteLine($"Queue2 max length: {Math.Round(Queue2MaxLength, 8)}");
        }
    }
}