using Microsoft.Playwright;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Bogus;

namespace Signupvalidations.Pages
{
    public abstract class SignupPageBase
    {
        protected readonly IPage _page;
        protected readonly IBrowser _browser;
        protected readonly IBrowserContext _context;
        protected readonly Faker _faker;
        protected static string _verificationCode = string.Empty;
        protected static bool _isConsoleListenerAttached = false;

        protected SignupPageBase(IPage page, IBrowser browser, IBrowserContext context)
        {
            _page = page ?? throw new ArgumentNullException(nameof(page));
            _browser = browser ?? throw new ArgumentNullException(nameof(browser));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _faker = new Faker();
            Task.Run(async () => await AttachConsoleListener()); // ✅ Await in Task.Run
        }

        public static async Task<T> CreateAsync<T>() where T : SignupPageBase
        {
            var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = false,
                SlowMo = 50
            });
            var context = await browser.NewContextAsync();
            var page = await context.NewPageAsync();

            var instance = Activator.CreateInstance(typeof(T), page, browser, context) as T;
            if (instance == null)
                throw new InvalidOperationException($"Unable to create an instance of {typeof(T).Name}");

            return instance;
        }

        protected async Task EnterVerificationOTPAsync()
        {
            Console.WriteLine("[Waiting for Verification Code input...]");
            var verificationInput = _page.GetByRole(AriaRole.Textbox, new() { Name = "Verification Code" });
            await verificationInput.WaitForAsync(new() { Timeout = 30000 });

            int retries = 30;
            while (retries-- > 0 && string.IsNullOrEmpty(_verificationCode))
            {
                await Task.Delay(1000);
            }

            if (string.IsNullOrEmpty(_verificationCode))
                throw new Exception("[Error] Verification code not captured from console logs.");

            Console.WriteLine($"[Entering Verification Code]: {_verificationCode}");
            await verificationInput.FillAsync(_verificationCode);
            await _page.GetByRole(AriaRole.Button, new() { Name = "OK" }).ClickAsync();
            await _page.GetByRole(AriaRole.Textbox, new() { Name = "Company Name" }).WaitForAsync(new() { Timeout = 30000 });
        }

        private async Task AttachConsoleListener()
        {
            if (_isConsoleListenerAttached) return;

            _page.Console += (_, msg) =>
            {
                Console.WriteLine($"[Console Log]: {msg.Text}");
                var match = Regex.Match(msg.Text, @"\b\d{4,6}\b");
                if (match.Success)
                {
                    _verificationCode = match.Value;
                    Console.WriteLine($"[Captured Verification Code]: {_verificationCode}");
                }
            };

            _isConsoleListenerAttached = true;
            await Task.CompletedTask; // ✅ Ensure async method has an await
        }
    }
}
