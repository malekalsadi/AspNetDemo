using Microsoft.AspNetCore.Mvc;
using AspNetDemo.Shared;
using AspNetDemo.Api.Models;

namespace AspNetDemo.Api.Services
{
    public interface IComplaintService
    {
        Task<RequestResponse> CreateComplaint(DemandModel[] demands, string userId);
        Task<List<Demand>> GetUserDemandsAsync(string userid);
        Task<List<Demand>> GetDemandsAsync(int? ComplaintId);
        Task<Demand> GetDemandDetailsAsync(int? DemandId);
        Task<RequestResponse> EditDemandAsync(Demand demand);
        Task<RequestResponse> DeleteDemandAsync(int? DemandId);
        Task<RequestResponse> DeleteComplaintAsync(int? ComplaintId);
        Task<List<Complaint>> GetAllComplaintsAsync();
        Task<List<Complaint>> GetUserComplaintAsync(string? userId);

    }
}
