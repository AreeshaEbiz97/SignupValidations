using NUnit.Framework;
using Microsoft.Playwright;
using System.Threading.Tasks;
using PlaywrightDotnetProject.Pages;

namespace PlaywrightDotnetProject.Tests
{
    public class SignupTest
    {
        private IPlaywright _playwright;
        private IBrowser _browser;
        private IPage _page;

        private SignupPage _signupPage;
        private AdministratorInfoPage _adminPage;
        private VerificationPage _verificationPage;
        private CompanySubscriptionPage _companySubscriptionPage;
        private BillingInfoPage _billingInfoPage;
        private PaymentInfoPage _paymentInfoPage;
        private PlaceOrderPage _placeOrderPage;
        private VerifyOrderSummaryPage _verifyOrderSummaryPage;

        [SetUp]
        public async Task Setup()
        {
            _playwright = await Playwright.CreateAsync();
            _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
            var context = await _browser.NewContextAsync();
            _page = await context.NewPageAsync();

            // Initialize all page classes
            _signupPage = new SignupPage(_page);
            _adminPage = new AdministratorInfoPage(_page);
            _verificationPage = new VerificationPage(_page);
            _companySubscriptionPage = new CompanySubscriptionPage(_page);
            _billingInfoPage = new BillingInfoPage(_page);
            _paymentInfoPage = new PaymentInfoPage(_page);
            _placeOrderPage = new PlaceOrderPage(_page);
            _verifyOrderSummaryPage = new VerifyOrderSummaryPage(_page);
        }

        [Test]
        public async Task CompleteSignupFlowTest()
        {
            await _signupPage.SignupAsync();
            await _adminPage.FillAdministratorInfoAsync();
            await _verificationPage.EnterVerificationOTPAsync();
            await _companySubscriptionPage.EnterCompanyDetailsAsync();
            await _billingInfoPage.BillingInfoAsync();
            await _paymentInfoPage.EnterPaymentInfoAsync();
            // await _placeOrderPage.ValidateHeadOfficeData();
            await _placeOrderPage.PlaceOrderAsync();
            await _verifyOrderSummaryPage.VerifyOrderSummaryAsync();
        }

        [TearDown]
        public async Task TearDown()
        {
            await _browser.CloseAsync();
            _playwright.Dispose();
        }
    }
}
