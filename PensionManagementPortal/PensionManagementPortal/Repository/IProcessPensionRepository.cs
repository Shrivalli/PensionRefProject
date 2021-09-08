using System;
using System.Threading.Tasks;
using PensionManagementPortal.Models;

namespace PensionManagementPortal.Repository
{
    public interface IProcessPensionRepository
    {
        Task<APIResponse<PensionDetail>> GetPensionDetail(PensionerInput pensionerInput);
        Task<APIResponse<ProcessPensionInfo>> ProcessPension(ProcessPensionInput processPensionInput);
    }
}
