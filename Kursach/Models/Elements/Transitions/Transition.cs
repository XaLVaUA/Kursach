using System.Collections.Generic;
using System.Linq;
using Kursach.Models.Elements.Positions;

namespace Kursach.Models.Elements.Transitions
{
    public class Transition : Element
    {
        private readonly SortedSet<double> _triggerTimes;
        private readonly int _mean;
        private readonly int _deviation;

        public ICollection<PositionArc> InArcs { get; }

        public double Ticks { get; set; }
        
        public double NextTriggerTime => _triggerTimes.Count != 0 ? _triggerTimes.Min : double.MaxValue;

        public bool IsActive => InArcs.All(arc => arc.IsActive);

        public bool IsTriggerAvailable => NextTriggerTime <= Ticks;
        
        public int Priority { get; }
        
        public Transition(string id, int mean, int deviation, int priority) : base(id)
        {
            _triggerTimes = new SortedSet<double>();
            _mean = mean;
            _deviation = deviation;
            InArcs = new List<PositionArc>();
            Ticks = 0L;
            Priority = priority;
        }

        public void TickMinus()
        {
            if (!IsActive) return;

            _triggerTimes.Add(Ticks + DelayCreator.CreateNextTimeDelay(_mean, _deviation));
            
            foreach (var arc in InArcs)
            {
                arc.Trigger();
            }
        }
        
        public void Trigger()
        {
            if (!IsTriggerAvailable) return;

            _triggerTimes.Remove(_triggerTimes.Min);
        }

        public TransitionArc Connect(Position position, int multiplicity = 1)
        {
            var arc = new TransitionArc($"{Id}-{position.Id}", multiplicity, this);
            
            position.InArcs.Add(arc);

            return arc;
        }
    }
}