using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OfficeNet.Domain.Contracts;
using OfficeNet.Domain.Entities;
using OfficeNet.Service.Thought;

namespace OfficeNet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ThoughtOfDayController : ControllerBase
    {
        private readonly IThoughtService _thoughtService;
        public ThoughtOfDayController(IThoughtService thoughtService)
        {
            _thoughtService = thoughtService;
        }

        [HttpPost("SaveThought")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> SaveThought(ThoughtSaveModel thoughtOfDay)
        {
            var flag = thoughtOfDay.ThoughtID;
            if (thoughtOfDay == null)
            {
                return BadRequest("Thought of the day cannot be null.");
            }
            try
            {
                var result = await _thoughtService.SaveThought(thoughtOfDay);
                if (flag == 0 && result.ThoughtID == 0)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Failed to save thought of the day.");
                }
                if (result.ThoughtID == flag)
                {
                    return StatusCode(StatusCodes.Status200OK, new
                    {
                        Message = "Thought of the day updated successfully.",
                        result
                    });
                }
                return StatusCode(StatusCodes.Status201Created, new
                {
                    Message = "Thought of the day saved successfully.",
                    ThoughtID = result.ThoughtID
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("GetThoughtOfTheDay")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetThoughtOfTheDay(bool Flag)
        {
            try
            {
                var result = await _thoughtService.GetThoughtOfTheDay(Flag);
                if (result == null || result.Count == 0)
                {
                    return Ok("No thoughts of the day found.");
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("TwoSum")]
        public int[] TwoSum(int[] nums, int target)
        {
            Dictionary<int, int> map = new Dictionary<int, int>();

            for (int i = 0; i < nums.Length; i++)
            {
                int complement = target - nums[i];
                if (map.ContainsKey(complement))
                {
                    return new int[] { map[complement], i };  
                }
                map[nums[i]] = i;
            }

            throw new ArgumentException("No two sum solution");
        }

    }
}
