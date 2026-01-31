using System.Net;
using System.Net.Mail;

public static async Task SendEmailAgent(string userEmail, Product product, string aiRecommendation)
{
    var smtpClient = new SmtpClient("myaccount.google.com")
    {
        Port = 587, // Replace with your SMTP server's port
        Credentials = new NetworkCredential("kubulee.kl@gmail.com", "your_password"), // Replace with your username and password
        EnableSsl = true,
    };

    string subject = $"SEEKIT Suggestion: {product.Name}";
    string body = $@"
        <html>
            <body style='font-family: Arial, sans-serif;'>
                <h2 style='color: #2D2D2D;'>SEEKIT Found Something for You!</h2>
                <p>Based on your search, here is a top pick:</p>
                <div style='border: 1px solid #ddd; padding: 15px; border-radius: 10px;'>
                    <h3>{product.Name}</h3>
                    <p><b>Price:</b> ${product.Price}</p>
                    <p>{product.Description}</p>
                </div>
                <br>
                <p><b>Why it was chosen:</b><br>{aiRecommendation}</p>
                <hr>
                <footer style='font-size: 0.8em; color: #888;'>Sent by SEEKIT Engine</footer>
            </body>
        </html>";

    var mailMessage = new MailMessage
    {
        From = new MailAddress("agent@seekit.ai", "SEEKIT"),
        Subject = subject,
        Body = body,
        IsBodyHtml = true,
    };
    mailMessage.To.Add(userEmail);

    try
    {
        Console.WriteLine($"[AGENT]: Delivering recommendation to {userEmail}...");
        await smtpClient.SendMailAsync(mailMessage);
        Console.WriteLine("[SUCCESS]: Email delivered!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[AGENT ERROR]: Could not send mail. {ex.Message}");
    }
}
