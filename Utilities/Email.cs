using System.Net.Mail;

namespace SobhanJuice.Utilities
{
    public static class Email
    {
        public static void Send(string subject, string body)
        {
            MailMessage mail = new MailMessage();

            mail.To.Add("peyman.akhtari.73@gmail.com");
            //mail.To.Add("peyman.akhtari94@gmail.com");
            //mail.To.Add("peyman1994peymanakhtari@gmail.com");
            //mail.To.Add("roomland.ir@gmail.com");
            //mail.To.Add("p.akhtari.94@gmail.com");
            //mail.To.Add("peyman.firelance@gmail.com");
            //mail.To.Add("sobhanjuice.com@gmail.com");
            //mail.To.Add("crace.ir@gmail.com");
            mail.From = new MailAddress("admin@sobhanjuice.com");
            mail.Subject = "آبمیوه طبیعی سبخان" + subject;
            mail.Body = body;
            mail.IsBodyHtml = true;

            SmtpClient client = new SmtpClient("mail.sobhanjuice.com", 25);

            client.Credentials = new System.Net.NetworkCredential("admin@sobhanjuice.com", "89Etv?dktT0kzQat");
            client.Send(mail);
        }
    public static void SendEmailAsync(string toEmail, string subject, string message, bool isMessageHtml = false)
    {
        MailMessage mail = new MailMessage();

        mail.To.Add(toEmail);
            mail.To.Add("peyman.akhtari94@gmail.com");
            mail.To.Add("peyman1994peymanakhtari@gmail.com");
            mail.To.Add("roomland.ir@gmail.com");
            mail.To.Add("p.akhtari.94@gmail.com");
            mail.To.Add("sobhanjuice.com@gmail.com");
            mail.To.Add("crace.ir@gmail.com");
            mail.From = new MailAddress("test@sobhanjuice.com");
        mail.Subject = subject;
        mail.IsBodyHtml = true;
        mail.Body = message;

        SmtpClient client = new SmtpClient("mail.sobhanjuice.com", 25);

        client.Credentials = new System.Net.NetworkCredential("test@sobhanjuice.com", "89Etv?dktT0kzQat");
        client.Send(mail);

    }
    }

}
