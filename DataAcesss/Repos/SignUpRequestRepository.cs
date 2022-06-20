using DataAcesss.Data;
using DataAcesss.Repos.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAcesss.Repos
{
    public class SignUpRequestRepository : ISignUpRequestRepository
    {
        private readonly PUSGS_ProjectContext _context;

        public SignUpRequestRepository(PUSGS_ProjectContext context)
        {
            _context = context;
        }

        public async Task Create(SignUpRequest request)
        {
            await _context.SignUpRequests.AddAsync(request);
            await _context.SaveChangesAsync();
        }

        public void Delete(int id)
        {
            var request = _context.SignUpRequests.FirstOrDefault(x => x.SignUpRequestId == id);
            if (request != null)
            {
                _context.SignUpRequests.Remove(request);
                _context.SaveChanges();
            }
        }

        public async Task DeleteAll()
        {
            foreach (var request in _context.SignUpRequests)
            {
                _context.Remove(request);
            }
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAll(string userId)
        {
            foreach (var request in _context.SignUpRequests)
            {
                if (request.AppUserId == userId)
                {
                    _context.Remove(request);
                }
            }
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAll(DateTime endDate)
        {
            foreach (var request in _context.SignUpRequests)
            {
                if (DateTime.Compare(request.Date, endDate) < 0)
                {
                    _context.Remove(request);
                }
            }
            await _context.SaveChangesAsync();
        }

        public async Task<SignUpRequest> Read(int id)
        {
            return await _context.SignUpRequests.FindAsync(id);
        }

        public List<SignUpRequest> ReadAll()
        {
            return _context.SignUpRequests.ToList();
        }

        public List<SignUpRequest> ReadAll(string userId)
        {
            return _context.SignUpRequests.Where(x => x.AppUserId == userId).ToList();
        }

        public List<SignUpRequest> ReadAll(SignUpRequestStatus status)
        {
            return _context.SignUpRequests.Where(x => x.Status == status).ToList();
        }

        public List<SignUpRequest> ReadAll(DateTime startDate)
        {
            return _context.SignUpRequests.Where(x => x.Date >= startDate).ToList();
        }

        public async Task Update(SignUpRequest request)
        {
            var updatedRequest = await _context.SignUpRequests.FindAsync(request.SignUpRequestId);
            updatedRequest.AppUserId = request.AppUserId;
            updatedRequest.Date = request.Date;
            updatedRequest.Status = request.Status;
            _context.SaveChanges();
        }
    }
}
