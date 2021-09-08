using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PensionerDetailService.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PensionerDetailService.Repository
{
    public class PensionerDetailsRepository : IPensionerDetailsRepository
    {
        private readonly ILogger<PensionerDetailsRepository> _logger;

        public PensionerDetailsRepository(ILogger<PensionerDetailsRepository> logger)
        {
            _logger = logger;
        }

        public PensionerDetail GetPensionerDetailsByAadhar(string aadhar)
        {
            List<PensionerDetail> pensionerDetails = GetDetailsCsv();

            return pensionerDetails.FirstOrDefault(s => s.AadharNumber == aadhar);
        }

        private List<PensionerDetail> GetDetailsCsv()
        {
            List<PensionerDetail> pensionerDetails = new List<PensionerDetail>();
            try
            {
                string fileName = "details.csv";
                _logger.LogInformation($"Try to open the file '{fileName}'");

                using (StreamReader sr = new StreamReader(fileName))
                {
                    _logger.LogInformation($"File opened successfully.Reading from the file.");

                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] values = line.Split(',');

                        pensionerDetails.Add(new PensionerDetail
                        {
                            Name = values[0].Trim(),
                            DateOfBirth = Convert.ToDateTime(values[1].Trim()),
                            PAN = values[2].Trim(),
                            AadharNumber = values[3].Trim(),
                            SalaryEarned = Convert.ToInt32(values[4].Trim()),
                            Allowances = Convert.ToInt32(values[5].Trim()),
                            PensionType = (PensionType)Enum.Parse(typeof(PensionType), values[6].Trim()),
                            BankDetail = new BankDetail
                            {
                                BankName = values[7].Trim(),
                                AccountNumber = values[8].Trim(),
                                BankType = (BankType)Enum.Parse(typeof(BankType), values[9].Trim())
                            }
                        });
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"{e.Message}");
                _logger.LogError($"{e.StackTrace}");

                return pensionerDetails;
            }

            return pensionerDetails;
        }
    }
}
