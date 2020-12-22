namespace Kursach.Models.Elements
{
    public abstract class Arc : Element
    {
        public abstract bool IsActive { get; }
        
        public int Multiplicity { get; }
        
        protected Arc(string id, int multiplicity) : base(id)
        {
            Multiplicity = multiplicity;
        }
    }
}