using System;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using PensionerDetailService.Models;
using PensionerDetailService.Repository;

namespace PensionerDetailService.Tests
{
    [TestFixture]
    public class PensionDetailsRepositoryTest
    {
        private PensionerDetailsRepository _repository;

        [SetUp]
        public void Setup()
        {
            Mock<ILogger<PensionerDetailsRepository>> mockLogger = new Mock<ILogger<PensionerDetailsRepository>>();
            _repository = new PensionerDetailsRepository(mockLogger.Object);
        }

        [TearDown]
        public void Teardown()
        {
            _repository = null;
        }

        [TestCase("987491728479")]
        [TestCase("100051726123")]
        [TestCase("511341223331")]
        [TestCase("103003726123")]
        [TestCase("638493766126")]
        [TestCase("876594318734")]
        public void GetPensionerDetailsByAadhar_ShouldRerutnNull_OnNonExistantAadharNumber(string aadhar)
        {
            // Act
            PensionerDetail pensionerDetail = _repository.GetPensionerDetailsByAadhar(aadhar);

            // Assert
            Assert.That(pensionerDetail, Is.Null);
        }


        [TestCase("111122223333")]
        [TestCase("212122223333")]
        [TestCase("511122223331")]
        [TestCase("564740730130")]
        [TestCase("638493726126")]
        [TestCase("876594358734")]
        public void GetPensionerDetailsByAadhar_ShouldRerutnPensionerDetail_OnValidAadharNumber(string aadhar)
        {
            // Act
            PensionerDetail pensionerDetail = _repository.GetPensionerDetailsByAadhar(aadhar);

            // Assert
            Assert.That(pensionerDetail, Is.InstanceOf<PensionerDetail>());
            Assert.That(pensionerDetail.AadharNumber, Is.EqualTo(aadhar));
        }
    }
}
