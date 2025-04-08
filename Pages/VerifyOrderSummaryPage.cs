using Microsoft.Playwright;
using System;
using System.Threading.Tasks;

namespace PlaywrightDotnetProject.Pages
{
    public class VerifyOrderSummaryPage
    {
        private readonly IPage _page;

        public VerifyOrderSummaryPage(IPage page)
        {
            _page = page;
        }

        public async Task VerifyOrderSummaryAsync()
        {
            Console.WriteLine("[Action]: Verifying order summary...");

            // Subscription Info
            var subscriptionLocator = _page.Locator("//*[@id='DivData']/table/tbody/tr[2]/td[1]/p");
            await subscriptionLocator.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 10000 });

            var subscriptionText = await subscriptionLocator.InnerTextAsync();
            if (string.IsNullOrEmpty(subscriptionText))
                throw new Exception("[Error]: Missing subscription info.");
            Console.WriteLine($"[Debug]: Subscription info: {subscriptionText}");

            // Admin Info
            var adminLocator = _page.Locator("//*[@id='DivData']/table/tbody/tr[2]/td[2]/p");
            await adminLocator.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 10000 });

            var adminText = await adminLocator.InnerTextAsync();
            if (string.IsNullOrEmpty(adminText))
                throw new Exception("[Error]: Missing admin info.");
            Console.WriteLine($"[Debug]: Admin info: {adminText}");

            Console.WriteLine("[Success]: Both subscription and admin info verified.");

            // Click on the "Get Started" button
            var getStartedButton = _page.GetByRole(AriaRole.Button, new() { Name = "Get Started" });
            await getStartedButton.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await getStartedButton.ClickAsync();

            Console.WriteLine("[Success]: Clicked on 'Get Started' button.");
        }
    }
}