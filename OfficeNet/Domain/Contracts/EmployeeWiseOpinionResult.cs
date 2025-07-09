using System.Reflection.Emit;

namespace OfficeNet.Domain.Contracts
{
    public class EmployeeWiseOpinionResult
    {
            public string EmpCode { get; set; }
            public string UserName  {get;set;}
            public string DeptName  {get;set;}
            public DateTime CreatedOn {get;set;}
            public string OptionName{get;set;}
    }
}
