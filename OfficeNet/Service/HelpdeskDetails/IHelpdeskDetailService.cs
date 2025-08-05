using OfficeNet.Domain.Contracts;

namespace OfficeNet.Service.HelpdeskDetails
{
    public interface IHelpdeskDetailService
    {
        Task<bool> CreateHelpdeskDetails(HelpDeskDetailDto helpDeskDetailDto);
    }
}
