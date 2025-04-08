using Microsoft.Playwright;
using System;
using System.Threading.Tasks;
using Bogus;

namespace PlaywrightDotnetProject.Pages
{
    public class AdministratorInfoPage
    {
        private readonly IPage _page;
        private readonly Faker _faker = new();

        public AdministratorInfoPage(IPage page)
        {
            _page = page;
        }

        public async Task FillAdministratorInfoAsync()
    { 
    Console.WriteLine("[INFO] Clicking 'Next' button to trigger validation...");
    await _page.GetByRole(AriaRole.Button, new() { Name = "Next" }).ClickAsync();

    // Wait for the error popup to appear
    Console.WriteLine("[INFO] Waiting for validation error message...");
    await _page.WaitForSelectorAsync("//div[contains(@class, 'sweet-alert') and contains(@class, 'visible')]");

    // Locate the error message text
    var errorMessageLocator = _page.Locator("//p[contains(@class, 'lead')]");

    // Extract text content safely
    var errorText = await errorMessageLocator.TextContentAsync() ?? string.Empty;

    Console.WriteLine($"[DEBUG] Captured error message: '{errorText}'");

    // Validate the expected message
    if (!errorText.Trim().Equals("Please fill in all the required fields.", StringComparison.OrdinalIgnoreCase))
    {
        throw new Exception($"Unexpected error message: '{errorText}'");
    }

    Console.WriteLine("[SUCCESS] Required Fields Validation Passed!");

    // Click the "OK" button to close the alert
    await PressOkButton();

    // Fill form with invalid email and verify error
            var fullName = _faker.Name.FullName();
            var phone = _faker.Phone.PhoneNumber("##########");
            var invalidEmail = "invalidEmail"; // Intentionally incorrect email format
            var password = "Aa1234567";

            await _page.GetByRole(AriaRole.Textbox, new() { Name = "Full Name" }).FillAsync(fullName);
            await _page.GetByRole(AriaRole.Textbox, new() { Name = "Phone" }).FillAsync(phone);
            await _page.GetByRole(AriaRole.Textbox, new() { Name = "Admin Email" }).FillAsync(invalidEmail);
            await _page.GetByRole(AriaRole.Textbox, new() { Name = "Password", Exact = true }).FillAsync(password);
            await _page.GetByRole(AriaRole.Textbox, new() { Name = "Confirm Password", Exact = true }).FillAsync(password);

            Console.WriteLine("[INFO] Clicking 'Next' button with invalid email...");
            await _page.GetByRole(AriaRole.Button, new() { Name = "Next" }).ClickAsync();
            await _page.WaitForTimeoutAsync(1000);

            // Expect field error for invalid email
          var modalSelector = "//div[contains(@class, 'sweet-alert') and contains(@class, 'visible')]";
          var modalMessageLocator = _page.Locator($"{modalSelector}//p[contains(@class, 'lead')]");

           var errorMessage = await modalMessageLocator.TextContentAsync() ?? string.Empty;

            // Check that the error message matches the expected "Invalid email."
            if (!errorMessage.Trim().Equals("Invalid email.", StringComparison.OrdinalIgnoreCase))
         {
             throw new Exception($"[ERROR] Unexpected error message: '{errorMessage}'");
         }
            await _page.GetByRole(AriaRole.Button, new() { Name = "OK" }).ClickAsync();

            await _page.WaitForSelectorAsync(modalSelector, new() { State = WaitForSelectorState.Detached, Timeout = 5000 });

             Console.WriteLine("[SUCCESS] Invalid Email Validation Passed!");

   var validEmail = _faker.Internet.Email();
Console.WriteLine($"[INFO] Generated valid email: {validEmail}");

// Ensure the email field is visible before filling
var emailField = _page.GetByRole(AriaRole.Textbox, new() { Name = "Admin Email" });
await emailField.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 5000 });
await emailField.FillAsync(validEmail);

Console.WriteLine("[SUCCESS] Email field updated successfully!");

// Wait for and Click the 'Next' button
var nextButton = _page.GetByRole(AriaRole.Button, new() { Name = "Next" });
await nextButton.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 5000 });
await nextButton.ClickAsync();
await Task.Delay(2000);
Console.WriteLine("[INFO] Clicked 'Next' button, waiting for response...");
await _page.Locator("button.confirm:has-text('OK')").ClickAsync(new LocatorClickOptions { Force = true });
    }

        private async Task PressOkButton()
{
    var okButton = _page.GetByRole(AriaRole.Button, new() { Name = "OK" });

    // Wait for the button to be visible and clickable
    await okButton.WaitForAsync(new() { State = WaitForSelectorState.Visible });

    await okButton.ClickAsync();
    Console.WriteLine("[INFO] Clicked 'OK' button.");
}


//  Helper method to verify validation messages
private async Task ExpectFieldErrorAsync(string fieldName)
{
    var errorLocator = _page.GetByText($"{fieldName} is required", new() { Exact = false });

    // Wait until error appears
    await errorLocator.WaitForAsync(new() { State = WaitForSelectorState.Visible });

    if (!await errorLocator.IsVisibleAsync())
    {
        throw new Exception($"Validation error for '{fieldName}' not found!");
    }
}
    }}