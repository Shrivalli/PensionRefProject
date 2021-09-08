using PensionerDetailService.Models;
using System;
using System.Collections.Generic;

namespace PensionerDetailService.Repository
{
    public interface IPensionerDetailsRepository
    {
        PensionerDetail GetPensionerDetailsByAadhar(string aadhar);
    }
}
