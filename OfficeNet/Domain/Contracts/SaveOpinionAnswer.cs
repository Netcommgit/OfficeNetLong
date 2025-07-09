namespace OfficeNet.Domain.Contracts
{
    public class SaveOpinionAnswer
    {
        public int QuestionId { get; set; }
        public string ModifiedBy { get; set; }

        public List<int> OptionIds { get; set; } = new List<int>();
    }
    


}
