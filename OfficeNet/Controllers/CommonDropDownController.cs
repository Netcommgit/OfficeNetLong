using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeNet.Service;
using OfficeNet.Service.Department;
using OfficeNet.Service.PlantService;

namespace OfficeNet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommonDropDownController : ControllerBase
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IPlantsMasterService _plantsMasterService;
        private readonly IDepartmentService _departmentService;

        public CommonDropDownController(ICurrentUserService currentUserService, IPlantsMasterService plantsMasterService, IDepartmentService departmentService)
        {
            _currentUserService = currentUserService;
            _plantsMasterService = plantsMasterService;
            _departmentService = departmentService;
        }   

        [HttpGet("GetPlantList")]
        [Authorize]
        public async Task<IActionResult> GetPlantList()
        {
          var result =  await  _plantsMasterService.GetPlantListAsync();

            var simplifiedResult = result
                .Select(p => new
                {
                    p.PlantId,
                    p.PlantName
                });
            return Ok(simplifiedResult);
        }
        [HttpGet("GetDepartmentList")]
        [Authorize]
        public async Task<IActionResult> GetDepartmentList()
        {
            var result = await _departmentService.GetDepartmentAsync();
            var simplifiedResult = result
            .Select(d => new
            {
                d.DeptID,
                d.DeptName
            });
            return Ok(simplifiedResult);
        }
    }
}
