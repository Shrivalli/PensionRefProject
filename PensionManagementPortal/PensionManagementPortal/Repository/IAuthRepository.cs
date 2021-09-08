using System;
using System.Threading.Tasks;
using PensionManagementPortal.Models;

namespace PensionManagementPortal.Repository
{
    public interface IAuthRepository
    {
        Task<APIResponse<AuthToken>> Login(UserCredential userCredential);
    }
}
