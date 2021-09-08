using ProcessPensionService.Models;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ProcessPensionService.Repository
{
    public interface IPensionDisbursementRepository
    {
        Task<ProcessPensionResponse> DisbursePension(ProcessPensionInput processPensionInput);
    }
}
