namespace Kursach.Models.Elements
{
    public abstract class Element
    {
        public string Id { get; }

        protected Element(string id)
        {
            Id = id;
        }
    }
}