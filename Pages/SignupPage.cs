using Microsoft.Playwright;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Bogus;

namespace PlaywrightDotnetProject.Pages
{
    public class SignupPage
    {
        private readonly IPage _page;
        private string? _verificationCode;
        private bool _isConsoleListenerAttached = false;
        private readonly Faker _faker = new();

        public SignupPage(IPage page)
        {
            _page = page;
            AttachConsoleListener();
        }

        private void AttachConsoleListener()
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

        public async Task SignupAsync()
        {
            await _page.GotoAsync("https://qasignup.e-bizsoft.net/Signup", new PageGotoOptions
            {
                WaitUntil = WaitUntilState.NetworkIdle,
                Timeout = 60000
            });
        }
    }
}
