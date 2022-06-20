using DataAcesss.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAcesss.Repos.Contracts
{
    public interface ISignUpRequestRepository
    {
        Task Create(SignUpRequest request);
        Task<SignUpRequest> Read(int id);
        List<SignUpRequest> ReadAll();
        List<SignUpRequest> ReadAll(string userId);
        List<SignUpRequest> ReadAll(SignUpRequestStatus status);
        List<SignUpRequest> ReadAll(DateTime startDate);
        Task Update(SignUpRequest request);
        void Delete(int id);
        Task DeleteAll();
        Task DeleteAll(string userId);
        Task DeleteAll(DateTime endDate);
    }
}
