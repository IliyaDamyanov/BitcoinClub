using BitcoinClub.Infrastructure.Files;
using Xunit;

namespace BitcoinClub.Tests.Files
{
    public class UploadPathValidatorTests
    {
        [Theory]
        [InlineData("a.png", true)]
        [InlineData("../a.png", false)]
        [InlineData("..\\a.png", false)]
        [InlineData("folder/a.png", false)]
        [InlineData("folder\\a.png", false)]
        public void IsAllowedFileName_RejectsPaths(string name, bool expected)
        {
            var v = new UploadPathValidator();
            Assert.Equal(expected, v.IsAllowedFileName(name));
        }

        [Theory]
        [InlineData(".png", true)]
        [InlineData("png", true)]
        [InlineData(".jpg", true)]
        [InlineData(".jpeg", true)]
        [InlineData(".webp", true)]
        [InlineData(".gif", false)]
        [InlineData("", false)]
        public void IsAllowedExtension_AllowsExpected(string ext, bool expected)
        {
            var v = new UploadPathValidator();
            Assert.Equal(expected, v.IsAllowedExtension(ext));
        }
    }
}
