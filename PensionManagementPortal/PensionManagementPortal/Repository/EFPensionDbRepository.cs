using System;
using System.Threading.Tasks;
using PensionManagementPortal.Models;
using PensionManagementPortal.Provider;

namespace PensionManagementPortal.Repository
{
    public class EFPensionDbRepository : IPensionDbRepository
    {
        private readonly IEFProvider _provider;
        public EFPensionDbRepository(IEFProvider provider)
        {
            _provider = provider;
        }

        public async Task AddProcessedPensionDetail(ProcessedPensionDetail processedPensionDetail)
        {
            await _provider.AddProcessedPensionDetail(processedPensionDetail);
        }
    }
}
