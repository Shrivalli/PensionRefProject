using System;
using System.Threading.Tasks;
using PensionManagementPortal.Models;

namespace PensionManagementPortal.Provider
{
    public interface IEFProvider
    {
        Task AddProcessedPensionDetail(ProcessedPensionDetail processedPensionDetail);
    }
}
