using AuthorizationService.Models;
using AuthorizationService.Repository;
using NUnit.Framework;
namespace AuthorizationService.Tests
{
    [TestFixture]
    public class AuthRepositoryTest
    {
        private AuthRepository _repository;

        [SetUp]
        public void Setup()
        {
            _repository = new AuthRepository();
        }

        [TearDown]
        public void TearDown()
        {
            _repository = null;
        }

        [TestCase("admin1", "123456")]
        [TestCase("admin2", "234567")]
        [TestCase("admin10", "101010")]
        [TestCase("admin20", "202020")]
        public void ValidateUserCredential_ShouldReturnTrue_WhenCredentialIsValid(string userName, string password)
        {
            // Arrange
            UserCredential credential = new UserCredential { UserName = userName, Password = password };

            // Act
            bool result = _repository.ValidateUserCredential(credential);

            // Assert
            Assert.That(result, Is.True);
        }

        [TestCase("admin1", "123416")]
        [TestCase("admin2", "235567")]
        [TestCase("admin10", "1012010")]
        [TestCase("admin20", "121234")]
        [TestCase("user1", "124r21")]
        [TestCase("user11", "12312e")]
        public void ValidateUserCredential_ShouldReturnFalse_WhenCredentialIsInvalid(string userName, string password)
        {
            // Arrange
            UserCredential credential = new UserCredential { UserName = userName, Password = password };

            // Act
            bool result = _repository.ValidateUserCredential(credential);

            // Assert
            Assert.That(result, Is.False);
        }
    }
}
