using Microsoft.Playwright;
using System;
using System.Threading.Tasks;
using PlaywrightDotnetProject.Pages;

namespace PlaywrightDotnetProject.Utils
{
    public class FormValidator
    {
        private readonly IPage _page;

        public FormValidator(IPage page)
        {
            _page = page;
        }

        public async Task VerifyEmptyFieldsValidationAsync()
        {
            Console.WriteLine("[INFO] Clicking 'Next' button to trigger validation...");
            await _page.GetByRole(AriaRole.Button, new() { Name = "Next" }).ClickAsync();

            // Wait for the error popup to appear
            Console.WriteLine("[INFO] Waiting for validation error message...");
            await _page.WaitForSelectorAsync("//div[contains(@class, 'sweet-alert') and contains(@class, 'visible')]");

            // Locate the error message text
            var errorMessageLocator = _page.Locator("//p[contains(@class, 'lead')]");
            var errorText = await errorMessageLocator.TextContentAsync() ?? string.Empty;

            Console.WriteLine($"[DEBUG] Captured error message: '{errorText}'");

            // Validate the expected message
            if (!errorText.Trim().Equals("Please fill in all the required fields.", StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception($"Unexpected error message: '{errorText}'");
            }

            Console.WriteLine("[SUCCESS] Required Fields Validation Passed!");

            // Click the "OK" button to close the alert
            await _page.WaitForTimeoutAsync(500);
            await PressOkButton();
        }

        public async Task PressOkButton()
        {
            var okButton = _page.GetByRole(AriaRole.Button, new() { Name = "OK" });

            // Wait for the button to be visible and clickable
            await okButton.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await _page.WaitForTimeoutAsync(1000);
            await okButton.ClickAsync();
            Console.WriteLine("[INFO] Clicked 'OK' button.");
            await _page.WaitForTimeoutAsync(2000);
            var modal = _page.Locator(".sweet-alert.showSweetAlert.visible");
            Console.WriteLine("[INFO] Waiting for modal to close...");
            await modal.WaitForAsync(new() { State = WaitForSelectorState.Hidden, Timeout = 5000 });

        }
                public async Task SwapSubsidiaryAsync()
        {
            Console.WriteLine("[INFO]: Attempting to swap subsidiary...");

            var secondSubsidiary = _page.Locator("#gridView tr").Nth(1); // Select second row

            // Check if row exists
            if (await secondSubsidiary.CountAsync() == 0)
            {
                Console.WriteLine("[ERROR]: Second subsidiary row not found!");
                throw new Exception("No second subsidiary available.");
            }

            // Click the Swap button
            await _page.Locator("#parentbutton").ClickAsync();
            Console.WriteLine("[INFO]: Clicked 'Swap HeadOffice!' Button.");

            // Wait for the swap confirmation modal
            var swapModal = _page.Locator(".sweet-alert.showSweetAlert.visible");
            await swapModal.WaitForAsync();

            // Click 'Swap!' button in modal
            await _page.Locator("button.confirm.btn-primary").ClickAsync();
            Console.WriteLine("[INFO]: Confirmed Swap in Modal.");

            // Wait for the final 'OK' confirmation button
            var okButton = _page.GetByText("OK", new() { Exact = true }).Nth(1);
            await okButton.WaitForAsync(new() { State = WaitForSelectorState.Visible });
            await okButton.ClickAsync();
            Console.WriteLine("[INFO]: Clicked final 'OK' button.");

            // Proceed with verification
            var verificationPage = new VerificationPage(_page);
            verificationPage.AttachConsoleListener();  
            await verificationPage.EnterVerificationOTPAsync();

            // Click "Next" button to proceed
            Console.WriteLine("[INFO]: Clicking 'Next' Button...");
            await _page.GetByRole(AriaRole.Button, new() { Name = "Next" }).ClickAsync();
        }

    }





}
