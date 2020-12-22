using System;

namespace Kursach.Models
{
    public static class DelayCreator
    {
        private static readonly Random Random = new();
        
        public static double CreateNextTimeDelay(int mean, int deviation)
        {
            return mean + Random.Next(-deviation, deviation);
        }
    }
}