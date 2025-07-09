namespace OfficeNet.Domain.Contracts
{
    public class OpinionResult
    {
      public string? Topic { get; set; } 
      public string? OptionName { get; set; } 
      public int? OptionCount { get; set; } 
      public decimal? OpinionPercentage { get; set; } 
                     
    }
}
