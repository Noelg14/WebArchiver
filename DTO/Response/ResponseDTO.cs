namespace WebArchiver.DTO.Response
{
    public class ResponseDTO<T>
    {
        public IList<T> Data { get; set; }
        public int TotalPages { get; set; }
        public bool MoreRecords { get; set; } = false;
    }
}
