using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.AspNet.Identity;
using reservation_booking_system.Entity_Framework;
using reservation_booking_system.ViewModels;


namespace reservation_booking_system.Controllers
{
    public class AccountController : Controller
    {

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Index",model);
            }
            ReservationSystemDBEntities reservationSystemDBEntities = new ReservationSystemDBEntities();
            var admindata = reservationSystemDBEntities.Admins.Where(x => x.Email == model.Email).Select(x => new { x.Name, x.ID,x.HashKey,x.HashedPassword }).FirstOrDefault();
            var clientdata = reservationSystemDBEntities.Clients.Where(x => x.Email == model.Email).FirstOrDefault();

            if (!(admindata == null))
            {
                var HashedPassword = HMACSHA256(model.Password, admindata.HashKey);
                if (HashedPassword == admindata.HashedPassword)
                {
                    // set id, name, role, accessLevel
                    UserData userData = new UserData
                    {
                        Name = admindata.Name,
                        Role = "admin",
                        AccessLevel = 2
                    };
                    var userdata = String.Join(",", userData.Name, userData.Role, userData.AccessLevel);
                    FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
                        1,
                        admindata.ID.ToString(),
                        DateTime.Now,
                        DateTime.Now.AddDays(20),
                        false,
                        userdata
                        );
                    // Encrypt the ticket.
                    string encTicket = FormsAuthentication.Encrypt(ticket);

                    // Create the cookie.
                    Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket));

                    return RedirectToAction("Index", "Home");

                }
                else
                {
                    ModelState.AddModelError("", "Incorrect email or password");
                    return View();
                }
              
            }
            else if(!(clientdata == null)) 
            {
                FormsAuthentication.SetAuthCookie(clientdata.ID.ToString(), false);
                // role = client
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("", "Incorrect email or password");
                return View();          
            }
            
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult RegisterClient(RegisterViewModel model)
        {
            ReservationSystemDBEntities reservationSystemDBEntities = new ReservationSystemDBEntities();
            
            Client client = new Client
            {
                Email = "fege",
                //var i = reservationSystemDBEntities.SaveChanges();
        };

            return View();
        }
       
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult RegisterAdmin(RegisterViewModel model)
        {
            ReservationSystemDBEntities reservationSystemDBEntities = new ReservationSystemDBEntities();
            var rnd = GenerateRandomString(25);
            Admin admin = new Admin
            {
                Email = model.Email,
                HashedPassword = HMACSHA256(model.Password, rnd),
                HashKey = rnd
            };
            
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logoff()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            return RedirectToAction("Index","Home");
        }

        [Authorize]
        public PartialViewResult HeaderLogin()
        {
            FormsIdentity user = (FormsIdentity) User.Identity;
            var struserdata = user.Ticket.UserData;
            var userdata = UserDatastr(struserdata);
            
            //FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(authCookie.Value);
            return PartialView("_HeaderLogin", userdata);
        }

        public static UserData UserDatastr(string strdata)
        {
            string[] subs = strdata.Split(',');
            var a = subs[2];
            UserData userData = new UserData()
            {
                Name = subs[0],
                Role = subs[1],
                AccessLevel = Int32.Parse(a)
            };
            return userData;
        }
        private static string HMACSHA256(string message, string key)
        {
            var encoding = new System.Text.UTF8Encoding();
            byte[] keyByte = encoding.GetBytes(key);
            byte[] messageBytes = encoding.GetBytes(message);
            using (var hmacSHA256 = new HMACSHA256(keyByte))
            {
                byte[] hashMessage = hmacSHA256.ComputeHash(messageBytes);
                return BitConverter.ToString(hashMessage).Replace("-", "").ToLower();
            }
        }
        public static string GenerateRandomString(int length, string allowableChars = null)
        {
            if (string.IsNullOrEmpty(allowableChars))
                allowableChars = @"ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890";

            // Generate random data
            var rnd = new byte[length];
            using (var rng = new RNGCryptoServiceProvider())
                rng.GetBytes(rnd);

            // Generate the output string
            var allowable = allowableChars.ToCharArray();
            var l = allowable.Length;
            var chars = new char[length];
            for (var i = 0; i < length; i++)
                chars[i] = allowable[rnd[i] % l];

            return new string(chars);
        }
    }
}
