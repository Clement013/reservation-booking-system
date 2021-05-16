using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;
using reservation_booking_system.Entity_Framework;
namespace reservation_booking_system.Mail
{
    public class MailServer
    {
        public static void SendEmail(string email,string sub,string body)
        {
            try
            {
                // retrive the data from config
                var sendemail = ConfigurationManager.AppSettings["emailsender"].ToString();
                var pass = ConfigurationManager.AppSettings["password"].ToString();
                var port = ConfigurationManager.AppSettings["portnumber"];
                var SMTP = ConfigurationManager.AppSettings["smtp"].ToString();
                var isSSL = ConfigurationManager.AppSettings["IsSSL"];

                // open mail and smtp
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient(SMTP);

                // parse data to mail
                mail.From = new MailAddress(sendemail);
                mail.To.Add(email);
                mail.Subject = sub;
                mail.IsBodyHtml = true;
                mail.Body = body;

                // parse mail to smtpserver to send
                SmtpServer.Port = Convert.ToInt16(port);
                SmtpServer.Credentials = new System.Net.NetworkCredential(sendemail, pass);
                SmtpServer.EnableSsl = Convert.ToBoolean(isSSL);

                // send email
                SmtpServer.Send(mail);
                
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }
        public static void ReservationsendClientEmail(Admin admindata,Event eventdata)
        {
            try
            {
                // create email templete for client                                                       
                string template = "D:\\Interview project\\reservation booking system\\reservation booking system\\Mail\\ClientReservetemplete.html";
                StreamReader str = new StreamReader(template);
                string htmltemplete = str.ReadToEnd();
                str.Close();
                htmltemplete = htmltemplete.Replace("[Name]", eventdata.Client.Name);
                htmltemplete = htmltemplete.Replace("[Title]", eventdata.Title);
                htmltemplete = htmltemplete.Replace("[Start]", eventdata.FromTime);
                htmltemplete = htmltemplete.Replace("[End]", eventdata.EndTime);
                htmltemplete = htmltemplete.Replace("[Description]", eventdata.Description);
                htmltemplete = htmltemplete.Replace("[Status]", eventdata.Approval);
                
                // create email subject for client
                var sub = "Reservation for " + admindata.Name;

                // send email
                var debug = Convert.ToBoolean(ConfigurationManager.AppSettings["debug"].ToString());
                if (debug)
                {
                    var devemail = ConfigurationManager.AppSettings["devemail"].ToString();
                    SendEmail(devemail, sub, htmltemplete);
                }
                else
                {
                    SendEmail(eventdata.Client.Email, sub, htmltemplete);
                }
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }
        public static void ReservationsendadminEmail(Admin admindata, Event eventdata)
        {
            try
            {
                // create email templete for admin                                                       
                string template = "D:\\Interview project\\reservation booking system\\reservation booking system\\Mail\\AdminReservetemplete.html";
                StreamReader str = new StreamReader(template);
                string htmltemplete = str.ReadToEnd();
                str.Close();
                htmltemplete = htmltemplete.Replace("[Name]", admindata.Name);
                htmltemplete = htmltemplete.Replace("[ClientName]", eventdata.Client.Name);
                htmltemplete = htmltemplete.Replace("[Title]", eventdata.Title);
                htmltemplete = htmltemplete.Replace("[Start]", eventdata.FromTime);
                htmltemplete = htmltemplete.Replace("[End]", eventdata.EndTime);
                htmltemplete = htmltemplete.Replace("[Description]", eventdata.Description);
                htmltemplete = htmltemplete.Replace("[Status]", eventdata.Approval);

                // create email subject for admin
                var sub = "New Reservation from " + eventdata.Client.Name;

                // send email
                var debug = Convert.ToBoolean(ConfigurationManager.AppSettings["debug"].ToString());
                if (debug)
                {
                    var devemail = ConfigurationManager.AppSettings["devemail"].ToString();
                    SendEmail(devemail, sub, htmltemplete);
                }
                else
                {
                    SendEmail(admindata.Email, sub, htmltemplete);
                }
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }
        public static void AproveReservationsendEmail(Admin admindata, Event eventdata)
        {
            try
            {
                // create email templete for client                                                       
                string template = "D:\\Interview project\\reservation booking system\\reservation booking system\\Mail\\ApproveReservation.html";
                
                StreamReader str = new StreamReader(template);
                string htmltemplete = str.ReadToEnd();
                str.Close();
                htmltemplete = htmltemplete.Replace("[Name]", eventdata.Client.Name);
                htmltemplete = htmltemplete.Replace("[Title]", eventdata.Title);
                htmltemplete = htmltemplete.Replace("[Start]", eventdata.FromTime);
                htmltemplete = htmltemplete.Replace("[End]", eventdata.EndTime);
                htmltemplete = htmltemplete.Replace("[Description]", eventdata.Description);
                htmltemplete = htmltemplete.Replace("[Status]", eventdata.Approval);

                // create email subject for client
                var sub = "Reservation Success for " + admindata.Name;

                // send email
                var debug = Convert.ToBoolean(ConfigurationManager.AppSettings["debug"].ToString());
                if (debug)
                {
                    var devemail = ConfigurationManager.AppSettings["devemail"].ToString();
                    SendEmail(devemail, sub, htmltemplete);
                }
                else
                {
                    SendEmail(eventdata.Client.Name, sub, htmltemplete);
                }
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }
        public static void CancelReservationsendEmail(Admin admindata, Event eventdata)
        {   
            try
            {
                // create email templete for client                                                       
                string template = "D:\\Interview project\\reservation booking system\\reservation booking system\\Mail\\CancelReservationtemplete.html";

                StreamReader str = new StreamReader(template);
                string htmltemplete = str.ReadToEnd();
                str.Close();
                htmltemplete = htmltemplete.Replace("[Name]", eventdata.Client.Name);
                htmltemplete = htmltemplete.Replace("[Title]", eventdata.Title);
                htmltemplete = htmltemplete.Replace("[Start]", eventdata.FromTime);
                htmltemplete = htmltemplete.Replace("[End]", eventdata.EndTime);
                htmltemplete = htmltemplete.Replace("[Description]", eventdata.Description);
                htmltemplete = htmltemplete.Replace("[adminName]", admindata.Name);
                htmltemplete = htmltemplete.Replace("[contact]", admindata.ContactNumber.ToString());


                // create email subject for client
                var sub = "Cancel Reservation for " + admindata.Name;

                // send email
                var debug = Convert.ToBoolean(ConfigurationManager.AppSettings["debug"].ToString());
                if (debug)
                {
                    var devemail = ConfigurationManager.AppSettings["devemail"].ToString();
                    SendEmail(devemail, sub, htmltemplete);
                }
                else
                {
                    SendEmail(eventdata.Client.Name, sub, htmltemplete);
                }
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }
    }
}