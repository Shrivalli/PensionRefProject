using ProcessPensionService.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProcessPensionService.Repository
{
    public interface IPensionerDetailRepository
    {
        Task<PensionerDetail> GetPensionerDetailByAadhar(string aadharNumber);
    }
}