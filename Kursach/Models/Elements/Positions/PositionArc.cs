namespace Kursach.Models.Elements.Positions
{
    public class PositionArc : Arc
    {
        private readonly Position _position;

        public override bool IsActive => _position.Markers >= Multiplicity;

        public PositionArc(string id, int multiplicity, Position position) : base(id, multiplicity)
        {
            _position = position;
        }

        public void Trigger()
        {
            if (!IsActive) return;
            
            _position.Markers -= Multiplicity;
        }
    }
}