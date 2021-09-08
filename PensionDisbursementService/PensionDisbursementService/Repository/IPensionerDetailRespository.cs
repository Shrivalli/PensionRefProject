using System;
using System.Threading.Tasks;
using PensionDisbursementService.Models;

namespace PensionDisbursementService.Repository
{
    public interface IPensionerDetailRespository
    {
        Task<PensionerDetail> GetPensionerDetailByAadhar(string aadhar);
    }
}
