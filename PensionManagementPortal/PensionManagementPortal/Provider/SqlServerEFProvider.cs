using System;
using System.Threading.Tasks;
using PensionManagementPortal.Models;

namespace PensionManagementPortal.Provider
{
    public class SqlServerEFProvider : IEFProvider
    {
        private readonly PensionContext _context;

        public SqlServerEFProvider(PensionContext context)
        {
            _context = context;
        }

        public async Task AddProcessedPensionDetail(ProcessedPensionDetail processedPensionDetail)
        {
            await _context.ProcessedPensionDetails.AddAsync(processedPensionDetail);
            await _context.SaveChangesAsync();
        }
    }
}
