using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using PensionManagementPortal.Models;
using PensionManagementPortal.Provider;
using PensionManagementPortal.Repository;

namespace PensionManagementPortal.Tests
{
    public class EFPensionDbRepositoryTest
    {
        private Mock<IEFProvider> _mockEFProvider;
        private EFPensionDbRepository _repository;

        [SetUp]
        public void Setup()
        {
            _mockEFProvider = new Mock<IEFProvider>();

            _repository = new EFPensionDbRepository(_mockEFProvider.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _repository = null;
        }

        [Test]
        public async Task AddProcessedPensionDetail_ShouldCall_AddProcessedPensionDetailOnce()
        {
            // Arrange
            _mockEFProvider.Setup(_ => _.AddProcessedPensionDetail(It.IsAny<ProcessedPensionDetail>())).Verifiable();

            // Act
            await _repository.AddProcessedPensionDetail(new ProcessedPensionDetail());

            // Assert
            _mockEFProvider.Verify(_ => _.AddProcessedPensionDetail(It.IsAny<ProcessedPensionDetail>()), Times.Once);
        }
    }
}
