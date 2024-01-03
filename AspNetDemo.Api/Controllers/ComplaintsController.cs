using AspNetDemo.Api.Models;
using AspNetDemo.Api.Services;
using AspNetDemo.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AspNetDemo.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ComplaintsController : Controller
    {
        private readonly IComplaintService _complaintService;
        public ComplaintsController(IComplaintService complaintService)
        {
            _complaintService = complaintService;
        }

        [HttpPost("Create")]
        public async Task<IActionResult> AddComplaint(DemandModel[] demands)
        {
            string userId = User.FindFirst("userId")?.Value;
            RequestResponse result = await _complaintService.CreateComplaint(demands,userId);
            if(result == null || result.code!=200) {
                return BadRequest();
            }
            return Ok();
        }
        [HttpGet("UserDemands")]
        public async Task<IActionResult> userComplaints()
        {
            string userId = User.FindFirst("userId")?.Value;
            var result = await _complaintService.GetUserDemandsAsync(userId);
            if(result == null)
            {
                return BadRequest();
            }

            return Ok(result);
        }

        [HttpGet("Details")]
        public async Task<IActionResult> ComplaintDetails(int? complaintId)
        {
            var result = await _complaintService.GetDemandsAsync(complaintId);
            if(result == null) 
            {
                return BadRequest();
            }

            return Ok(result);
        }

        [HttpGet("DemandDetails")]
        public async Task<IActionResult> DemandDetails(int? demandId)
        {
            Demand demand = await _complaintService.GetDemandDetailsAsync(demandId);
            if (demand == null)
                return BadRequest();
            return Ok(demand);
        }

        [HttpPut("Edit")]
        public async Task<IActionResult> EditDemand(Demand demand)
        {
            var result = await _complaintService.EditDemandAsync(demand);
            if (result.code != 200) return BadRequest(result);
            return Ok(result);
        }

        [HttpDelete("DeleteDemand")]
        public async Task<IActionResult> DeleteDemand(int? DemandId)
        {
            RequestResponse result = await _complaintService.DeleteDemandAsync(DemandId);
            if(result.code != 200) return BadRequest();
            return Ok(result);
            
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> DeleteComplaint(int? ComplaintId)
        {
            RequestResponse result = await _complaintService.DeleteComplaintAsync(ComplaintId);
            return Ok(result);
        }

        [HttpGet("AllComplaints")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllComplaints()
        {
            List<Complaint> allComplaints = await _complaintService.GetAllComplaintsAsync();
            if (allComplaints == null)
                return BadRequest();
            return Ok(allComplaints);
        }

    }
}
