using AutoMapper;
using DataAcesss.Data;
using DataAcesss.Repos.Contracts;
using ServiceLayer.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Services
{
    public class SignUpRequestService : ISignUpRequestService
    {
        private readonly IMapper _mapper;
        private readonly ISignUpRequestRepository _requestRepository;

        public SignUpRequestService(IMapper mapper, ISignUpRequestRepository requestRepository)
        {
            _mapper = mapper;
            _requestRepository = requestRepository;
        }

        public void CreateRequest(string userId)
        {
            var request = new SignUpRequest();
            request.Date = DateTime.Now;
            request.Status = SignUpRequestStatus.PENDING;
            request.AppUserId = userId;
            _requestRepository.Create(request);
        }

        public List<SignUpRequest> GetAllRequests()
        {
            return _requestRepository.ReadAll();
        }

        public async Task Update(int id, SignUpRequestStatus status)
        {
            var request = await _requestRepository.Read(id);
            request.Status = status;
            await _requestRepository.Update(request);
        }

        public SignUpRequestStatus? GetLatestRequestStatus(string userId)
        {
            var requests = _requestRepository.ReadAll(userId);
            if (requests != null && requests.Count > 0)
            {
                if (requests.Count == 1)
                {
                    return requests[0].Status;
                }

                // Sort by date and return latest
                requests = requests.OrderBy(x => x.Date).ToList();
                return requests[0].Status;
            }
            return null;
        }
    }
}
