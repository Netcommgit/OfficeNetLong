//using OfficeNet.Migrations;
using System.ComponentModel.Design;

namespace OfficeNet.Domain.Contracts
{
    public class ActivatePollDTO
    {
        public int QuestionId { get; set; }
        public bool ShowResults { get; set; }
        public string Topic { get; set; }
        public bool SelectionType { get; set; }
        public int PollOptionId { get; set; }
        public string? OptionName { get; set; }
        public int? OptioinId { get; set; }
    }
}
