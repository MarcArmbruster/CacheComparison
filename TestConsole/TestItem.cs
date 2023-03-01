namespace TestConsole
{
    public class TestItem
    {
        public int Id { get; set; }

        public string Name { get; set; }
            
        public object? Tag { get; set; }

        public TestItem(int id, string name)
        {
            this.Id = id;
            this.Name = name;
        }

        public TestItem(int id, string name, object? tag)
        {
            this.Id = id;
            this.Name = name;
            this.Tag = tag;
        }
    }
}
