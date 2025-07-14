using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Infrastructure;
using OfficeNet.Domain.Contracts;
using OfficeNet.Domain.Entities;
using OfficeNet.Infrastructure.Context;
//using OfficeNet.Migrations;
using OfficeNet.Service;
using OfficeNet.Service.Department;

namespace OfficeNet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentService _departmentService;
        private readonly ICurrentUserService _currentUserService;

        public DepartmentController(IDepartmentService departmentService, ICurrentUserService currentUserService)
        {
            _departmentService = departmentService;
            _currentUserService = currentUserService;
        }
        
        [Authorize(Policy = "Department.Create")]
        [HttpPost("SaveDepartmentData")]
        public async Task<IActionResult> SaveDepartmentData(DepartmentDto department)
        {
            if (department == null)
            {
                return BadRequest();
            }
            try
            {
                //department.CreatedBy = _currentUserService.GetUserId();
                //department.CreatedOn = DateTime.Now;
                //department.ModifiedBy = _currentUserService.GetUserId();
                //department.ModifiedOn = DateTime.Now;
                var result = await _departmentService.SaveDepartmentAsync(department);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpPut("UpdateDepartment")]
        [Authorize(Policy = "Department.Update")]
        public async Task<IActionResult> UpdateDepartment(UsersDepartment department)
        {
            if (department == null || department.DeptID == null || department.DeptID == 0)
                { return BadRequest(); }
            department.ModifiedBy = _currentUserService.GetUserId();
            department.ModifiedOn = DateTime.Now;
            var result = await _departmentService.UpdateDepartmentAsync(department);
            return Ok(result);
        }
        [HttpGet("GetDepartment")]
        [Authorize]
        public async Task<IActionResult> GetDepartmentList()
        {
            var result =  await _departmentService.GetDepartmentAsync();
            return Ok(result);
        }
    }
}
