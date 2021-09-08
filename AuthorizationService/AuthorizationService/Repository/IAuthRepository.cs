using System;
using AuthorizationService.Models;

namespace AuthorizationService.Repository
{
    public interface IAuthRepository
    {
        bool ValidateUserCredential(UserCredential userCredential);
    }
}
