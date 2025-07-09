namespace OfficeNet.Domain.Contracts
{
    public class EmailRequest
    {
        public List<string> To { get; set; } = new();
        public List<string>? Cc { get; set; }
        public List<string>? Bcc { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool IsHtml { get; set; } = true;
        public Dictionary<string, string>? Headers { get; set; }
    }

}
