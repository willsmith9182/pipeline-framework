namespace Pipeline.Definition
{
    public class Pipeline
    {
        public Pipeline(string name)
        {
            Name = name;
            Modules = new Modules();
        }

        public string Name { get; private set; }
        public bool InvokeAll { get; set; }
        public Modules Modules { get; private set; }
    }
}