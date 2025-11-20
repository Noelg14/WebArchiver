namespace WebArchiver.Entities
{
    public class Pages
    {
        public Pages() { }

        public Pages(string id, string url, string content, DateTime created)
        {
            Id = id;
            URL = url;
            Content = content;
            Created = created;
        }

        public required string Id { get; set; }
        public required string URL { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime Created { get; set; } = DateTime.Now;
    }
}
