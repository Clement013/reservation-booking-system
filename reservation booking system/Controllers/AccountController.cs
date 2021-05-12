using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
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
            var admindata = reservationSystemDBEntities.Admins.Where(x => x.Email == model.Email && x.Status == 1).Select(x => new { x.Name, x.ID,x.HashKey,x.HashedPassword }).FirstOrDefault();
            var clientdata = reservationSystemDBEntities.Clients.Where(x => x.Email == model.Email || x.UserName == model.Email && x.Status == 1).Select(x => new { x.Name,x.UserName, x.ID, x.HashedKey, x.HashedPassword }).FirstOrDefault();

            // admin side
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

            // client side
            else if (!(clientdata == null)) 
            {
                var HashedPassword = HMACSHA256(model.Password, clientdata.HashedKey);
                if (HashedPassword == clientdata.HashedPassword)
                {
                    // set id, name, role, accessLevel
                    UserData userData = new UserData
                    {
                        Name = clientdata.Name,
                        Role = "client",
                        AccessLevel = 1
                    };
                    var userdata = String.Join(",", userData.Name, userData.Role, userData.AccessLevel);
                    FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
                        1,
                        clientdata.UserName,
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
            else
            {
                ModelState.AddModelError("", "Incorrect email or password");
                return View();          
            }
            
        }
        public ActionResult Register()
        {

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model)
        {
            ReservationSystemDBEntities reservationSystemDBEntities = new ReservationSystemDBEntities();
            var cldata = reservationSystemDBEntities.Clients.Where(x => x.Email == model.Email || x.UserName == model.userName).FirstOrDefault();
            if (cldata == null)
            {

                var rnd = GenerateRandomString(25);
                Client client = new Client
                {
                    UserName = model.userName,
                    Name = model.Name,
                    Email = model.Email,
                    HashedPassword = HMACSHA256(model.Password, rnd),
                    HashedKey = rnd,
                    ContactNumber = model.Contact,
                    CreatedBy = model.userName,
                    CreatedTime = DateTime.Now.ToString(),
                    UpdatedBy = model.userName,
                    UpdatedTime = DateTime.Now.ToString(),
                    Status = 1

                };
                reservationSystemDBEntities.Clients.Add(client);
                reservationSystemDBEntities.SaveChanges();
                return RedirectToAction("Login");
            }
            else
            {
                ModelState.AddModelError("", "User Name or Email Exists");
                return View();
            }
        }

        public ActionResult RegisterAdmin() 
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult RegisterAdmin(RegisterAdViewModel model)
        {

            ReservationSystemDBEntities reservationSystemDBEntities = new ReservationSystemDBEntities();
            var amdata = reservationSystemDBEntities.Admins.Where(x => x.Email == model.Email).FirstOrDefault();
            if (amdata == null)
            {
                
                var rnd = GenerateRandomString(25);
                Admin admin = new Admin
                {
                    Name = model.Name,
                    Email = model.Email,
                    HashedPassword = HMACSHA256(model.Password, rnd),
                    HashKey = rnd,
                    ContactNumber = model.Contact,
                    CreatedTime = DateTime.Now.ToString(),
                    UpdatedTime = DateTime.Now.ToString(),
                    Status = 1
                    
                };
                reservationSystemDBEntities.Admins.Add(admin);
                reservationSystemDBEntities.SaveChanges();
                return RedirectToAction("Login");
            }
            else
            {
                ModelState.AddModelError("", "Email Exists");
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logoff()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            return RedirectToAction("Index","Home");
        }

        [AcceptVerbs("Get","Post")]
        [AllowAnonymous]
        public JsonResult IsEmailUsed(string email)
        {
            var data = "";
            using (var aa = new ReservationSystemDBEntities())
            {
                data = aa.Database.SqlQuery<string>("select Email from Admin ad where ad.Email = @email Union select Email from Client ct where ct.Email = @email", new SqlParameter("@email", email)).FirstOrDefault();
            }
            if (data == null)
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            
        }

        [AcceptVerbs("Get", "Post")]
        [AllowAnonymous]
        public JsonResult IsUserNameUsed(string username)
        {

            ReservationSystemDBEntities reservationSystemDBEntities = new ReservationSystemDBEntities();
            var ctdata = reservationSystemDBEntities.Clients.Where(x => x.UserName == username).FirstOrDefault();

            if (ctdata == null)
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

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
