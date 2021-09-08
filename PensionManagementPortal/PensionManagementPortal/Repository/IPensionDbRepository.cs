using System;
using System.Threading.Tasks;
using PensionManagementPortal.Models;

namespace PensionManagementPortal.Repository
{
    public interface IPensionDbRepository
    {
        Task AddProcessedPensionDetail(ProcessedPensionDetail processedPensionDetail);
    }
}
