using AspNetDemo.Api.Models;
using AspNetDemo.Shared;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AspNetDemo.Api.Services
{
    public class ComplaintService : IComplaintService
    {
        private readonly AppDbContext _dbContext;
        public ComplaintService(AppDbContext dbContext) 
        {
            _dbContext = dbContext;
        }
        public async Task<RequestResponse> CreateComplaint(DemandModel[] demands, string userId)
        {
            if (demands == null)
                return new RequestResponse
                {
                    code = 404,
                    message = "demands are Null"
                };

            Complaint complaint = new Complaint();
            complaint.CreatedDate = DateTime.Now;
            complaint.UserId = userId;
            var result = await _dbContext.AddAsync(complaint);
            await _dbContext.SaveChangesAsync();

            int len = demands.Length;
            for(int i = 0;i < len; i++)
            {
                Demand demand = new Demand
                {
                    Name = demands[i].Name,
                    Description = demands[i].Description,
                    ComplaintId = complaint.Id
                };
                var result2 = await _dbContext.AddAsync(demand);
                await _dbContext.SaveChangesAsync();
            }
            return new RequestResponse
            {
                message = "complaint created successfuly",
                code = 200
            };
        }

        public async Task<List<Demand>> GetUserDemandsAsync(string userId)
        {
            try
            {
                List<Complaint> complaints = await GetUserComplaintAsync(userId);

                List<Demand> result = new List<Demand>();
                foreach (Complaint complaint in complaints)
                {
                    List<Demand> result2 = await GetDemandsAsync(complaint.Id);
                    result.AddRange(result2);
                }

                return result;
                
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<List<Demand>> GetDemandsAsync(int? complaintId)
        {
            try
            {
                var complaintDemands = await _dbContext.demands.Where(c => c.ComplaintId == complaintId).ToListAsync();
                return complaintDemands;
            }
            catch(Exception ex)
            {
                return null;
            }
        }
        public async Task<Demand> GetDemandDetailsAsync(int? DemandId)
        {
            var result = await _dbContext.demands.FirstOrDefaultAsync(d => d.Id == DemandId);
            return result;
        }
        public async Task<RequestResponse> EditDemandAsync(Demand demand)
        {
            try { 
                var result = _dbContext.Update(demand);
                await _dbContext.SaveChangesAsync();
                return new RequestResponse
                {
                    code = 200,
                    message = "Update success"
                };
            }catch(Exception ex)
            {
                return new RequestResponse
                {
                    code = 500,
                    message = ex.Message
                };
            }
        }
        public async Task<RequestResponse> DeleteDemandAsync(int? DemandId)
        {
            Demand demand = await GetDemandDetailsAsync(DemandId);
            if (demand == null)
            {
                return new RequestResponse
                {
                    code = 404,
                    message = "Demand not found"
                };
            }
            _dbContext.Remove(demand); _dbContext.SaveChanges();
            if (await GetDemandDetailsAsync(DemandId) != null)
                return new RequestResponse
                {
                    code = 409,
                    message = "Delete failed"
                };
            return new RequestResponse { code = 200, message = "OK" };
        }
        public async Task<RequestResponse> DeleteComplaintAsync(int? ComplaintId)
        {
            if (await _dbContext.complaints.FirstOrDefaultAsync(c => c.Id == ComplaintId) == null)
                return new RequestResponse
                {
                    code = 406,
                    message = "Complaint not found"
                };

            List<Demand> demands = await GetDemandsAsync(ComplaintId);
            foreach (Demand demand in demands)
            {
                _dbContext.Remove(demand); 
                await _dbContext.SaveChangesAsync();
            }

            Complaint complaint = await _dbContext.complaints.FirstOrDefaultAsync(c => c.Id == ComplaintId);
            _dbContext.Remove(complaint); _dbContext.SaveChanges();

            return new RequestResponse { code = 200, message = "OK" };
        }

        public async Task<List<Complaint>> GetUserComplaintAsync(string? UserId)
        {
            var userComplaints = _dbContext.complaints
                    .Where(c => c.UserId == UserId)
                    .ToList();

            return userComplaints;
        }
        public async Task<List<Complaint>> GetAllComplaintsAsync()
        {
            var allComplaints = _dbContext.complaints.ToList();
            return allComplaints;
        }
    }
}
