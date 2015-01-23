namespace NaiveBayes.Data
{
    public class Class
    {
        protected bool Equals(Class other)
        {
            return string.Equals(Name, other.Name);
        }

        public override int GetHashCode()
        {
            return (Name != null ? Name.GetHashCode() : 0);
        }

        public string Name { get; set; }       
    }
}
