using Microsoft.Playwright;
using System;
using System.Threading.Tasks;
using PlaywrightDotnetProject.Utils;

namespace PlaywrightDotnetProject.Pages
{
    public class BillingInfoPage
    {
        private readonly IPage _page;
        private readonly FormValidator _formValidator;

        public BillingInfoPage(IPage page)
        {
            _page = page;
            _formValidator = new FormValidator(_page); // Initialize FormValidator
        }

        public async Task BillingInfoAsync()
        {
            Console.WriteLine("[Action]: Starting Billing Information process...");
            try
            {
                // Dismiss alerts
                if (await _page.Locator(".modal").IsVisibleAsync())
                {
                    Console.WriteLine("[DEBUG] Closing modal...");
                    await _page.Locator(".modal-close-button").ClickAsync();
                    await _page.WaitForTimeoutAsync(1000); // Allow modal to close properly
                }

                // **Ensure overlay is hidden**
                var overlay = _page.Locator("div#overlay");
                if (await overlay.IsVisibleAsync())
                {
                    Console.WriteLine("[Info]: Waiting for overlay to hide.");
                    await overlay.WaitForAsync(new() { State = WaitForSelectorState.Hidden, Timeout = 15000 });
                }

                // **Locate the checkbox**
                var billingCheckbox = _page.Locator("input#chkBillingSameAsCompany").First;
                await billingCheckbox.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 10000 });
                await billingCheckbox.ScrollIntoViewIfNeededAsync();

                // **Retry clicking checkbox**
                for (int attempt = 0; attempt < 3; attempt++)
                {
                    if (!await billingCheckbox.IsCheckedAsync())
                    {
                        try
                        {
                            Console.WriteLine($"[Attempt {attempt + 1}] Clicking 'Billing Same As Company' checkbox...");
                            await billingCheckbox.CheckAsync(new() { Force = true });
                            await Task.Delay(500);
                            if (await billingCheckbox.IsCheckedAsync()) break; // Success
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"[Warning] Click failed: {ex.Message}. Retrying...");
                        }
                    }
                    else
                    {
                        Console.WriteLine("[Info]: 'Billing Same As Company' checkbox was already checked.");
                        break;
                    }
                }

                Console.WriteLine("[Action]: Billing Information process completed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] An error occurred in BillingInfoAsync: {ex.Message}");
                throw; // Rethrow for debugging
            }
            var nextButton = _page.GetByRole(AriaRole.Button, new() { Name = "Next" });
            await nextButton.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 5000 });
            await nextButton.ClickAsync();
            await Task.Delay(2000);
            Console.WriteLine("[INFO] Clicked 'Next' button, waiting for response...");
        }
    }
}
