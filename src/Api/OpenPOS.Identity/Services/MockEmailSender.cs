using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace OpenPOS.Identity.Services
{
    public class MockEmailSender : IEmailSender
    {
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            await Task.Delay(200);
        }
    }
}