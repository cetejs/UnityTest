namespace GameFramework
{
    public abstract class SubWindow
    {
        public string Name { get; private set; }
        public GameWindow Parent { get; private set; }

        public virtual void Init(string name, GameWindow parent)
        {
            Name = name;
            Parent = parent;
        }

        public virtual void OnGUI()
        {
        }

        public virtual void OnDestroy()
        {
        }
    }
}
