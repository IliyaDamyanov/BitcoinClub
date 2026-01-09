using BitcoinClub.Controllers;
using BitcoinClub.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace BitcoinClub.Tests.Landing
{
    public class HomeControllerLandingTests
    {
        [Fact]
        public void Index_ReturnsView_WithLandingPageViewModel()
        {
            var controller = new HomeController(NullLogger<HomeController>.Instance);

            var result = controller.Index();

            var view = Assert.IsType<ViewResult>(result);
            Assert.IsType<LandingPageViewModel>(view.Model);
        }
    }
}
