    using Microsoft.Playwright;
    using System;
    using System.Threading.Tasks;
    using Bogus;

    namespace PlaywrightDotnetProject.Pages
    {
        public class CompanySubscriptionPage
        {
            private readonly IPage _page;
             private string? _verificationCode;
             private bool _isConsoleListenerAttached = false;
            private readonly Faker _faker = new();

            public CompanySubscriptionPage(IPage page)
            {
                _page = page;
            }
            public static class DataLogger
            {
                 private static readonly string logFilePath = "CompanyDataLog.txt";

                 public static void LogData(string data)
            {
                File.AppendAllText(logFilePath, $"{DateTime.Now}: {data}{Environment.NewLine}");
            }
}
            public async Task EnterCompanyDetailsAsync()
            { 
                
        // Click 'Next' button to trigger validation
            await _page.GetByRole(AriaRole.Button, new() { Name = "Next" }).ClickAsync();
            Console.WriteLine("[INFO] Clicked 'Next' button.");

        // Step 1: Wait for the error message element to be visible
            var errorMessageLocator = _page.Locator("p.lead.text-muted");
            await errorMessageLocator.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 50000 });
            await Task.Delay(2000);

            string? errorMessage = await errorMessageLocator.TextContentAsync();
            if (errorMessage != null && errorMessage.Contains("Please complete all required fields before adding the company."))
        {
         Console.WriteLine("[SUCCESS] Correct error message is displayed: " + errorMessage);
        }
        else
        {
             Console.WriteLine("[ERROR] Incorrect error message displayed: " + errorMessage);
        }

            await _page.Locator("button.confirm").ClickAsync(new() { Force = true });
            Console.WriteLine("[INFO] Clicked 'OK' button.");
            await Task.Delay(2000);


           Console.WriteLine("[INFO]: Creating Parent Company");

            var parentCompanyData = GenerateCompanyData();
            var parentAdminData = GenerateAdminData();
            DataLogger.LogData($"[Parent Company] Name: {parentCompanyData.name}, Phone: {parentCompanyData.phone}, City: {parentCompanyData.city}");
            DataLogger.LogData($"[Parent Admin] Name: {parentAdminData.fullName}, Email: {parentAdminData.email}");
            await FillCompanyForm(parentCompanyData);
            await FillAdminForm(parentAdminData, isParent: true);
            await _page.GetByRole(AriaRole.Button, new() { Name = "Next" }).ClickAsync();

    // Click "Yes" for Subsidiary
    await ClickYesForSubsidiary();

    // **Create only one subsidiary with a different admin**
    Console.WriteLine("[INFO]: Creating Subsidiary");
    var subsidiaryData = GenerateCompanyData();
    var subsidiaryAdminData = GenerateAdminData();
    DataLogger.LogData($"[Subsidiary] Name: {subsidiaryData.name}, Phone: {subsidiaryData.phone}, City: {subsidiaryData.city}");
    DataLogger.LogData($"[Subsidiary Admin] Name: {subsidiaryAdminData.fullName}, Email: {subsidiaryAdminData.email}");
    await FillCompanyForm(subsidiaryData);
    await FillAdminForm(subsidiaryAdminData, isParent: false);

    // Click "Add Company"
    await _page.GetByRole(AriaRole.Button, new() { Name = "Add Company" }).ClickAsync();
    await Task.Delay(3000);

    Console.WriteLine("[SUCCESS]: Parent Company and One Subsidiary Created Successfully.");


    
    
    //  add swap funcationality 
         var secondSubsidiary = _page.Locator("#gridView tr").Nth(1); // Select second row

    // Check if row exists
    if (await secondSubsidiary.CountAsync() == 0)
    {
        Console.WriteLine("[ERROR]: Second subsidiary row not found!");
        throw new Exception("No second subsidiary available.");
    }

    // Click the Swape button inside the row
    await _page.Locator("#parentbutton").ClickAsync();
    Console.WriteLine("[INFO]: Clicked 'Swap HeadOffice!' Button.");

    // Wait for the confirmation modal to appear
    var swapModal = _page.Locator(".sweet-alert.showSweetAlert.visible");
    await swapModal.WaitForAsync();

    // Click the 'Swap!' button in the modal
    await _page.Locator("button.confirm.btn-primary").ClickAsync();
    Console.WriteLine("[INFO]: Confirmed Swap in Modal.");

    // Wait for the next confirmation button
    var okButton2 = _page.GetByText("OK", new() { Exact = true }).Nth(1);
    await okButton2.WaitForAsync(new() { State = WaitForSelectorState.Visible });
    await okButton2.ClickAsync();
    
    await _page.GetByRole(AriaRole.Button, new() { Name = "Next" }).ClickAsync();
    await _page.WaitForTimeoutAsync(1000);
   
    // Okay Button
    var okButton = _page.GetByRole(AriaRole.Button, new() { Name = "OK" });

    // Wait for the button to be visible and clickable
    await okButton.WaitForAsync(new() { State = WaitForSelectorState.Visible });

    await okButton.ClickAsync();
    Console.WriteLine("[INFO] Clicked 'OK' button.");

    // **Call your functions after OK button is clicked**
        var verificationPage = new VerificationPage(_page);
        verificationPage.AttachConsoleListener();  // Console listener ko attach karo
        await verificationPage.EnterVerificationOTPAsync();

     // Click "Next" button to proceed
    Console.WriteLine("[INFO]: Clicking 'Next' Button...");
    await _page.GetByRole(AriaRole.Button, new() { Name = "Next" }).ClickAsync();
}

// **Utility Functions**
private async Task FillCompanyForm((string name, string phone, string address, string city, string zip) companyData)
{
    await _page.GetByRole(AriaRole.Textbox, new() { Name = "Company Name" }).FillAsync(companyData.name);
    await _page.Locator("#txtmultiPhone").FillAsync(companyData.phone);
    await _page.GetByRole(AriaRole.Textbox, new() { Name = "Address Line 1" }).FillAsync(companyData.address);
    await _page.GetByRole(AriaRole.Textbox, new() { Name = "City" }).FillAsync(companyData.city);
    await _page.Locator("#cbomultistate").SelectOptionAsync(new[] { "195" });
    await _page.GetByRole(AriaRole.Textbox, new() { Name = "Zip/Postal Code" }).FillAsync(companyData.zip);
    await _page.Locator("#ddlmultiChooseApplication").SelectOptionAsync("2");
    await _page.GetByRole(AriaRole.Textbox, new() { Name = "0" }).FillAsync("5");
    await _page.Locator("div#overlay").WaitForAsync(new() { State = WaitForSelectorState.Hidden });
}

private async Task FillAdminForm((string fullName, string contact, string email, string password) adminData, bool isParent)
{
    var adminCheckbox = _page.GetByRole(AriaRole.Checkbox, new() { Name = "Different admin info for" });

    if (!isParent) // Only check for different admins
    {
        if (!(await adminCheckbox.IsCheckedAsync()))
        {
            await adminCheckbox.CheckAsync();
        }
        // Click "Add Company" without filling admin fields
        await _page.GetByRole(AriaRole.Button, new() { Name = "Add Company" }).ClickAsync();
        await Task.Delay(3000);

        // Verify validation message appears
        var validationMessage = await _page.Locator("p.lead.text-muted").InnerTextAsync();
        if (validationMessage.Contains("Please complete all required fields before adding the company."))
    {
         Console.WriteLine("[Validation]: Required field message verified.");
        await _page.Locator("button.confirm.btn.btn-lg.btn-primary").ClickAsync(); // Click "Okay" button
    }
        await _page.GetByRole(AriaRole.Textbox, new() { Name = "Full Name" }).FillAsync(adminData.fullName);
        await _page.Locator("#txtAdminContact").FillAsync(adminData.contact);
        await _page.GetByRole(AriaRole.Textbox, new() { Name = "Admin Email" }).FillAsync(adminData.email);
        await _page.GetByRole(AriaRole.Textbox, new() { Name = "Password", Exact = true }).FillAsync(adminData.password);
        await _page.GetByRole(AriaRole.Textbox, new() { Name = "Confirm Password" }).FillAsync(adminData.password);
    }
      await _page.Locator("div#overlay").WaitForAsync(new() { State = WaitForSelectorState.Hidden });}
        private async Task ClickYesForSubsidiary()
        {
        var yesButton = _page.GetByRole(AriaRole.Button, new() { Name = "Yes" });
        await yesButton.WaitForAsync(new() { State = WaitForSelectorState.Visible });
        await Task.Delay(1000);
        await yesButton.ScrollIntoViewIfNeededAsync();
        await yesButton.ClickAsync(new() { Force = true });

    Console.WriteLine("[Modal]: Clicked on 'Yes' button.");

        await Task.WhenAll(
        _page.Locator("div.sweet-alert").WaitForAsync(new() { State = WaitForSelectorState.Hidden }),
        _page.Locator(".sweet-overlay").WaitForAsync(new() { State = WaitForSelectorState.Hidden })
     );
    }

// **Data Generators**
private (string name, string phone, string address, string city, string zip) GenerateCompanyData()
{
    return (
        _faker.Company.CompanyName(),
        _faker.Phone.PhoneNumber("##########"),
        _faker.Address.StreetAddress(),
        _faker.Address.City(),
        _faker.Address.ZipCode()
    );
}

private (string fullName, string contact, string email, string password) GenerateAdminData()
{
    return (
        _faker.Name.FullName(),
        _faker.Phone.PhoneNumber("##########"),
        _faker.Internet.Email(),
        "Aa1234567"
    );
        }
    }
}

        
    
    