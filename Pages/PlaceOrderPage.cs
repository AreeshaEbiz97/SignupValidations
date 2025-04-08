using Microsoft.Playwright;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace PlaywrightDotnetProject.Pages
{
    public class PlaceOrderPage
    {
        private readonly IPage _page;

        public PlaceOrderPage(IPage page)
        {
            _page = page;
        }

                public async Task PlaceOrderAsync()
{
    var overlay = _page.Locator("#overlay");
    if (await overlay.IsVisibleAsync())
        await overlay.WaitForAsync(new() { State = WaitForSelectorState.Hidden });

    var understandCheckbox = _page.Locator("#chkUnderstand").First;
    await understandCheckbox.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 10000 });
    await understandCheckbox.ScrollIntoViewIfNeededAsync();

    if (!await understandCheckbox.IsCheckedAsync())
        await understandCheckbox.CheckAsync(new() { Force = true });

    var placeOrderButton = _page.GetByRole(AriaRole.Button, new() { Name = "Place Order" });
    await placeOrderButton.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 10000 });
    await placeOrderButton.ScrollIntoViewIfNeededAsync();
    await placeOrderButton.ClickAsync(new() { Force = true });

    Console.WriteLine("[Success]: Order placed successfully.");
}

    //         public async Task ValidateHeadOfficeData()
    // {
    //    Console.WriteLine("[SUCCESS]: start headoffice summary  successfully.");
    //         await _page.WaitForFunctionAsync("document.querySelector('#ContentPlaceHolder1_lblCompanyName')?.innerText.length > 0");
    //         Console.WriteLine("1");
    //         await _page.WaitForFunctionAsync("document.querySelector('#ContentPlaceHolder1_lblAddress')?.innerText.length > 0");
    //         Console.WriteLine("2");
    //         // await _page.WaitForSelectorAsync("#ContentPlaceHolder1_lblAddress2");
    //         // await _page.WaitForSelectorAsync("#ContentPlaceHolder1_lblFieldsInfo");
    //         // await _page.WaitForSelectorAsync("#ContentPlaceHolder1_lblCountry");
    //         // await _page.WaitForSelectorAsync("#ContentPlaceHolder1_lblPhone");

    //         var companyName = await _page.Locator("#ContentPlaceHolder1_lblCompanyName").InnerTextAsync();
    //         var address1 = await _page.Locator("#ContentPlaceHolder1_lblAddress").InnerTextAsync();
    //         // var companyName = await _page.Locator("#ContentPlaceHolder1_lblCompanyName").InnerTextAsync();
    //         // var address1 = await _page.Locator("#ContentPlaceHolder1_lblAddress").InnerTextAsync();
    //         // var address2 = await _page.Locator("#ContentPlaceHolder1_lblAddress2").InnerTextAsync();
    //         // var stateZip = await _page.Locator("#ContentPlaceHolder1_lblFieldsInfo").InnerTextAsync();
    //         // var country = await _page.Locator("#ContentPlaceHolder1_lblCountry").InnerTextAsync();
    //         // var phone = await _page.Locator("#ContentPlaceHolder1_lblPhone").InnerTextAsync();

    //     // Read expected data from log file
    //     string[] expectedData = System.IO.File.ReadAllLines("CompanyDataLog.txt");

    //     // Assertions to validate extracted data with expected data
    //     Assert.Multiple(() =>
    //     {
    //         Assert.That(companyName, Is.EqualTo(expectedData[0]), "Company Name does not match!");
    //         Assert.That(address1, Is.EqualTo(expectedData[1]), "Address 1 does not match!");
    //         // Assert.That(address2, Is.EqualTo(expectedData[2]), "Address 2 does not match!");
    //         // Assert.That(stateZip, Is.EqualTo(expectedData[3]), "State and Zip do not match!");
    //         // Assert.That(country, Is.EqualTo(expectedData[4]), "Country does not match!");
    //         // Assert.That(phone, Is.EqualTo(expectedData[5]), "Phone number does not match!");
    //     });
    // }
    //         public async Task PlaceOrderAsync()
    //     {
    //         var overlay = _page.Locator("#overlay");
    //         if (await overlay.IsVisibleAsync())
    //             await overlay.WaitForAsync(new() { State = WaitForSelectorState.Hidden });

    //         var placeOrderButton = _page.GetByRole(AriaRole.Button, new() { Name = "Place Order" });
    //         await placeOrderButton.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 10000 });
    //         await placeOrderButton.ScrollIntoViewIfNeededAsync();
    //         await placeOrderButton.ClickAsync(new() { Force = true });

    //         Console.WriteLine("[SUCCESS]: Order placed successfully.");
    //     }
    }
}
