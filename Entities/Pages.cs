namespace WebArchiver.Entities
{
    public class Pages
    {
        public Pages()
        {
        }

        public Pages(string id, string uRl, string content, DateTime created)
        {
            Id = id;
            URl = uRl;
            Content = content;
            Created = created;
        }

        public string Id { get; set; }
        public string URl { get; set; }
        public string Content { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
    }
}
