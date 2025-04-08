using Microsoft.Playwright;
using System;
using System.Threading.Tasks;
using Bogus;

namespace PlaywrightDotnetProject.Pages
{
    public class PaymentInfoPage
    {
        private readonly IPage _page;
        private readonly Faker _faker = new();

        public PaymentInfoPage(IPage page)
        {
            _page = page;
        }

        public async Task EnterPaymentInfoAsync()
        {
            Console.WriteLine("[Action]: Filling Payment Information...");
            
            var accountTitle = _faker.Name.FullName();
            var accountNumber = _faker.Finance.Account();
            var routingNumber = _faker.Random.ReplaceNumbers("#########");

            await _page.GetByRole(AriaRole.Textbox, new() { Name = "Account Title" }).FillAsync(accountTitle);
            await _page.GetByRole(AriaRole.Textbox, new() { Name = "Account Number" }).FillAsync(accountNumber);
            await _page.Locator("#ddlAccountType").SelectOptionAsync(new SelectOptionValue { Label = "Checking Account" });
            await _page.GetByRole(AriaRole.Textbox, new() { Name = "Routing/ABA Number" }).FillAsync(routingNumber);

            var reviewCheckbox = _page.GetByRole(AriaRole.Checkbox, new() { Name = "Please review and accept ACH" });
            await reviewCheckbox.WaitForAsync(new() { State = WaitForSelectorState.Visible });
            await reviewCheckbox.ScrollIntoViewIfNeededAsync();

            if (!await reviewCheckbox.IsCheckedAsync())
            {
                await reviewCheckbox.CheckAsync(new() { Force = true });
            }

            Console.WriteLine("[Action]: Checked 'Please review and accept ACH'.");

            Console.WriteLine("[INFO]: Clicking 'Next' Button...");
            await _page.GetByRole(AriaRole.Button, new() { Name = "Next" }).ClickAsync();

        

            Console.WriteLine("[Success]: Payment Information submitted successfully.");
        }

    }
}