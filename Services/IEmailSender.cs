using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeziRotasi.Services
{
    public interface IEmailSender
    {
        Task SendAsync(string toEmail, string subject, string htmlBody, CancellationToken ct = default);

    }
}