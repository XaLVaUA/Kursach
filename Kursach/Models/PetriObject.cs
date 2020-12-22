using System.Collections.Generic;
using System.Linq;
using Kursach.Models.Elements.Positions;
using Kursach.Models.Elements.Transitions;

namespace Kursach.Models
{
    public class PetriObject
    {
        public IDictionary<string, Position> Positions { get; }
        
        public IDictionary<string, Transition> Transitions { get; }

        public double NextTriggerTime => Transitions.Values.Min(transition => transition.NextTriggerTime);
        
        public PetriObject()
        {
            Positions = new Dictionary<string, Position>();
            Transitions = new Dictionary<string, Transition>();
        }

        public void TickMinus(double ticks)
        {
            foreach (var transition in Transitions.Values.OrderByDescending(transition => transition.Priority))
            {
                transition.TickMinus();
            }
        }

        public void TickPlus(double ticks)
        {
            foreach (var transition in Transitions.Values)
            {
                transition.Ticks = ticks;
            }
            
            foreach (var position in Positions.Values)
            {
                position.TickPlus();
            }

            foreach (var transition in Transitions.Values)
            {
                transition.Trigger();
            }
        }
        
        public Position CreatePosition(string id)
        {
            var position = new Position(id);
            
            Positions.Add(position.Id, position);

            return position;
        }

        public Transition CreateTransition(string id, int mean, int deviation, int priority = 0)
        {
            var transition = new Transition(id, mean, deviation, priority);

            Transitions.Add(transition.Id, transition);

            return transition;
        }
    }
}