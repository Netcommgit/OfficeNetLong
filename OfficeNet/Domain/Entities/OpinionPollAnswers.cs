using Microsoft.EntityFrameworkCore;

namespace OfficeNet.Domain.Entities
{
    [Keyless]
    public class OpinionPollAnswer
    {
        public int QuestionId { get; set; }
        public int OptionId { get; set; }
        public int OptionCount { get; set; }
    }
}
