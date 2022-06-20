using DataAcesss.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Contracts
{
    public interface ISignUpRequestService
    {
        void CreateRequest(string userId);
        List<SignUpRequest> GetAllRequests();

        Task Update(int id, SignUpRequestStatus status);

        SignUpRequestStatus? GetLatestRequestStatus(string userId);
    }
}
