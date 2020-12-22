using System.Collections.Generic;
using System.Linq;
using Kursach.Models.Elements.Transitions;

namespace Kursach.Models.Elements.Positions
{
    public class Position : Element
    {
        public ICollection<TransitionArc> InArcs { get; }
        
        public int Markers { get; set; }

        public Position(string id) : base(id)
        {
            InArcs = new List<TransitionArc>();
        }

        public void TickPlus()
        {
            foreach (var arc in InArcs.Where(arc => arc.IsActive))
            {
                Markers += arc.Multiplicity;
            }
        }

        public PositionArc Connect(Transition transition, int multiplicity = 1)
        {
            var arc = new PositionArc($"{Id}-{transition.Id}", multiplicity, this);
            
            transition.InArcs.Add(arc);

            return arc;
        }
    }
}