using System;
using System.Collections.Generic;
using System.Linq;
using Kursach.Models.Statistics;

namespace Kursach.Models
{
    public class Model
    {
        private readonly IDictionary<string, PetriObject> _petriObjects;
        private readonly ICollection<PositionStatistics> _positionsStatisticsCollection;
        private readonly ICollection<ResultStatistics> _resultStatisticsCollection;
        private double _ticksLimit;
        private int _iterations;
        
        private bool _isInitialized;

        public Model()
        {
            _petriObjects = new Dictionary<string, PetriObject>();
            _positionsStatisticsCollection = new List<PositionStatistics>();
            _resultStatisticsCollection = new List<ResultStatistics>();
        }
        
        public void Initialize()
        {
            if (_isInitialized)
            {
                return;
            }

            _ticksLimit = 10000;
            _iterations = 2;

            const int n1 = 5;
            const int n2 = 20;
            const int n3 = 5;

            const int meanT1 = 5;
            const int meanT2 = 20;
            const int meanT3 = 10;

            const int devT1 = 1;
            const int devT2 = 7;

            // init create1
            {
                var create = new PetriObject();

                var p1 = create.CreatePosition("p1");
                var p2 = create.CreatePosition("p2");
                var t1 = create.CreateTransition("t1", meanT1, devT1);

                p1.Connect(t1);
                t1.Connect(p1);
                t1.Connect(p2, n1);

                p1.Markers = 1;
                
                _petriObjects.Add("create1", create);
            }

            // init create2
            {
                var create = new PetriObject();

                var p1 = create.CreatePosition("p1");
                var p2 = create.CreatePosition("p2");
                var t1 = create.CreateTransition("t1", meanT2, devT2);

                p1.Connect(t1);
                t1.Connect(p1);
                t1.Connect(p2, n2);

                p1.Markers = 1;

                _petriObjects.Add("create2", create);
            }

            // init create3
            {
                var create = new PetriObject();

                var p1 = create.CreatePosition("p1");
                var p2 = create.CreatePosition("p2");
                var t1 = create.CreateTransition("t1", meanT3, 0);

                p1.Connect(t1);
                t1.Connect(p1);
                t1.Connect(p2);

                p1.Markers = 1; 

                _petriObjects.Add("create3", create);
            }

            // init process
            {
                var process = new PetriObject();

                var p1 = _petriObjects["create1"].Positions["p2"];
                var p2 = _petriObjects["create2"].Positions["p2"];
                var p3 = _petriObjects["create3"].Positions["p2"];
                
                process.Positions.Add("p1", p1);
                process.Positions.Add("p2", p2);
                process.Positions.Add("p3", p3);

                var p4 = process.CreatePosition("p4");
                var p5 = process.CreatePosition("p5");
                var p6 = process.CreatePosition("p6");
                var p7 = process.CreatePosition("p7");

                var t1 = process.CreateTransition("t1", 0, 0);
                var t2 = process.CreateTransition("t2", 0, 0, 1);
                var t3 = process.CreateTransition("t3", 0, 0);

                p1.Connect(t1, n3);
                p2.Connect(t1, n3);
                t1.Connect(p4);
                p4.Connect(t2);
                t2.Connect(p5);
                p3.Connect(t2);
                p3.Connect(t3);
                t3.Connect(p6);
                p7.Connect(t1);
                t2.Connect(p7);

                p7.Markers = 1;
                
                _petriObjects.Add("process", process);
            }

            _isInitialized = true;
        }
        
        public void Start()
        {
            if (!_isInitialized)
            {
                return;
            }

            Console.WriteLine($"Iterations: {_iterations}");
            Console.WriteLine($"Ticks Limit: {_ticksLimit}");

            for (int i = 0; i < _iterations; ++i)
            {
                var currentTicks = 0d;

                SaveStatistics(currentTicks);

                while (currentTicks < _ticksLimit)
                {
                    foreach (var petriObject in _petriObjects.Values)
                    {
                        petriObject.TickMinus(currentTicks);
                    }

                    currentTicks = _petriObjects.Values.Min(petriObject => petriObject.NextTriggerTime);

                    foreach (var petriObject in _petriObjects.Values)
                    {
                        petriObject.TickPlus(currentTicks);
                    }

                    SaveStatistics(currentTicks);
                }

                CalculateStatistics();

                Console.WriteLine();
                _resultStatisticsCollection.Last().Print();
                Console.WriteLine();
            }

            PrintAverageStatistics();
        }

        private void SaveStatistics(double ticks)
        {
            var positionsStatistics =
                _petriObjects
                    .Select
                    (
                        petriObject => 
                        petriObject.Value.Positions
                            .Select(position => new PositionStatistics
                            {
                                Ticks = ticks,
                                PetriObjectId = petriObject.Key,
                                PositionId = position.Value.Id,
                                Markers = position.Value.Markers
                            })
                    )
                    .SelectMany(x => x);

            foreach (var positionStatistics in positionsStatistics)
            {
                _positionsStatisticsCollection.Add(positionStatistics);
            }
        }

        private void CalculateStatistics()
        {
            var grouped =
                _positionsStatisticsCollection
                    .GroupBy(x => x.PetriObjectId)
                    .Select
                    (
                        x =>
                            new
                            {
                                PetriObjectId = x.Key,
                                Positions =
                                    x
                                        .GroupBy(y => y.PositionId)
                                        .ToDictionary(y => y.Key, y => y.ToList())
                            }
                    )
                    .ToDictionary(x => x.PetriObjectId, x => x.Positions);

            var lastP5Markers = grouped["process"]["p5"][^1].Markers;
            var lastP6Markers = grouped["process"]["p6"][^1].Markers;
            var idleProbability = (double)lastP6Markers / lastP5Markers;

            var create1P2Statistics = grouped["create1"]["p2"].GroupBy(x => x.Ticks).Select(x => x.Last()).ToList();

            var queue1Average = create1P2Statistics.Aggregate(((double Average, double LastTicks, int LastMarkers))(0, 0, 0), (res, item) =>
            {
                res.Average += (double)res.LastMarkers * (item.Ticks - res.LastTicks);
                res.LastTicks = item.Ticks;
                res.LastMarkers = item.Markers;
                
                return res;
            }).Average / _ticksLimit;

            var queue1Max = create1P2Statistics.Max(x => x.Markers);
            var queue1Sum = create1P2Statistics.Sum(x => x.Markers);

            var create2P2Statistics = grouped["create2"]["p2"].GroupBy(x => x.Ticks).Select(x => x.Last()).ToList();

            var queue2Average = create2P2Statistics.Aggregate(((double Average, double LastTicks, int LastMarkers))(0, 0, 0), (res, item) =>
            {
                res.Average += (double)res.LastMarkers * (item.Ticks - res.LastTicks);
                res.LastTicks = item.Ticks;
                res.LastMarkers = item.Markers;

                return res;
            }).Average / _ticksLimit;

            var queue2Max = create2P2Statistics.Max(x => x.Markers);
            var queue2Sum = create2P2Statistics.Sum(x => x.Markers);

            _resultStatisticsCollection.Add(new ResultStatistics
            {
                IdleProbability = idleProbability,
                Queue1AverageLength = queue1Average,
                Queue1MaxLength = queue1Max,
                Queue2AverageLength = queue2Average,
                Queue2MaxLength = queue2Max
            });
        }

        private void PrintAverageStatistics()
        {
            var averageIdleProbability = _resultStatisticsCollection.Average(x => x.IdleProbability);
            var averageQueue1AverageLength = _resultStatisticsCollection.Average(x => x.Queue1AverageLength);
            var averageQueue1MaxLength = _resultStatisticsCollection.Average(x => x.Queue1MaxLength);
            var averageQueue2AverageLength = _resultStatisticsCollection.Average(x => x.Queue2AverageLength);
            var averageQueue2MaxLength = _resultStatisticsCollection.Average(x => x.Queue2MaxLength);

            Console.WriteLine($"Averages");
            Console.WriteLine($"Idle probability: {Math.Round(averageIdleProbability, 8)}");
            Console.WriteLine($"Queue1 average length: {Math.Round(averageQueue1AverageLength, 8)}");
            Console.WriteLine($"Queue1 max length: {Math.Round(averageQueue1MaxLength, 8)}");
            Console.WriteLine($"Queue2 average length: {Math.Round(averageQueue2AverageLength, 8)}");
            Console.WriteLine($"Queue2 max length: {Math.Round(averageQueue2MaxLength, 8)}");
        }
    }
}