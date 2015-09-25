
namespace AOP.UnitTests
{
    public class Pet
    {
        public virtual string Name { get; set; }
        public virtual int Age { get; set; }
        public virtual bool Deceased { get; set; }

        public override string ToString()
        {
            return string.Format("Name: {0}, Age: {1}, Deceased: {2}", Name, Age, Deceased);
        }
    }
}
