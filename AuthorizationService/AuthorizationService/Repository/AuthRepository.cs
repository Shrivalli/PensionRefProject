using System;
using System.Collections.Generic;
using AuthorizationService.Data;
using AuthorizationService.Models;

namespace AuthorizationService.Repository
{
    public class AuthRepository : IAuthRepository
    {
        public bool ValidateUserCredential(UserCredential userCredential)
        {
            Dictionary<string, string> credentails = Credentials.AdminCredentials;

            if (credentails.ContainsKey(userCredential.UserName) && credentails[userCredential.UserName] == userCredential.Password)
                return true;

            return false;
        }
    }
}
