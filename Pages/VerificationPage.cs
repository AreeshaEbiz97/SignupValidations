using Microsoft.Playwright;
using NUnit.Framework;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PlaywrightDotnetProject.Pages
{
    public class VerificationPage
    {
        private readonly IPage _page;
        private string? _verificationCode;
        private bool _isConsoleListenerAttached = false;

        public VerificationPage(IPage page)
        {
            _page = page;
            AttachConsoleListener();
        }

        public void AttachConsoleListener()
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
        }

        public async Task EnterVerificationOTPAsync()
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

            // Click OK button (first and second instance)
            var okButtons = _page.GetByRole(AriaRole.Button, new() { Name = "OK" });
            await okButtons.First.WaitForAsync(new() { Timeout = 5000 });
            await okButtons.First.ClickAsync();
            await okButtons.Nth(1).ClickAsync();

            // Wait for the next page (Company Name input)
            await _page.GetByRole(AriaRole.Textbox, new() { Name = "Company Name" }).WaitForAsync(new() { Timeout = 30000 });
        } 
    }
}
