using BitcoinClub.Areas.Admin.ViewModels;
using Xunit;

namespace BitcoinClub.Tests.SocialMedia
{
    public class PostCreateViewModelTests
    {
        [Fact]
        public void Defaults_AreInitialized()
        {
            var vm = new PostCreateViewModel();

            Assert.NotNull(vm.Images);
            Assert.NotNull(vm.SelectedPlatforms);
            Assert.NotNull(PostCreateViewModel.SupportedPlatforms);
        }
    }
}
