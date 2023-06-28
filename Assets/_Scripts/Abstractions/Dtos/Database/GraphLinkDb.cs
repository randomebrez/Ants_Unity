namespace Assets.Dtos.Database
{
    public class GraphLinkDb
    {
        public int Id { get; set; }

        public int OriginId { get; set; }

        public int TargetId { get; set; }

        public int GraphId { get; set; }

        public string LinkType { get; set; }
    }
}
