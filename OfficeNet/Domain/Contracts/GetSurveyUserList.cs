//using OfficeNet.Migrations;
using System.Reflection.Emit;

namespace OfficeNet.Domain.Contracts
{
    public class GetSurveyUserList
    {
            public string EmpCode              {get;set;}
            public string EmployeeName         {get;set;}
            public string Department           {get;set;}
            public string? Designation          {get;set;}
            public string Location             {get;set;}
            public string Status               {get;set;}
            public string? SubmissionDateTime   {get;set;}
    }
}
