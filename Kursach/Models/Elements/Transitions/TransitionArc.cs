namespace Kursach.Models.Elements.Transitions
{
    public class TransitionArc : Arc
    {
        private readonly Transition _transition;

        public override bool IsActive => _transition.IsTriggerAvailable;
        
        public TransitionArc(string id, int multiplicity, Transition transition) : base(id, multiplicity)
        {
            _transition = transition;
        }

        public void Trigger()
        {
            if (!IsActive) return;
            
            _transition.Trigger();
        }
    }
}