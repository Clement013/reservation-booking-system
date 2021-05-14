using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using reservation_booking_system.Entity_Framework;
using reservation_booking_system.ViewModels;

namespace reservation_booking_system.Controllers
{
    public class HomeController : Controller
    {
        [Route("home/index/{id:regex(\\d{4})}")]
        public ActionResult Index(int id)
        {
            if(Request.IsAuthenticated)
            {
                FormsIdentity user = (FormsIdentity)User.Identity;
                var struserdata = user.Ticket.UserData;
                var userdata = AccountController.UserDatastr(struserdata);
                // admin 
                if (userdata.AccessLevel == 2)
                {
                    ReservationSystemDBEntities reservationSystemDBEntities = new ReservationSystemDBEntities();
                    List<Client> Client = new List<Client>();
                    var clientdata = reservationSystemDBEntities.Clients.Where(x => x.Status == 1).ToList();
                    foreach(var sub in clientdata)
                    {
                        Client client = new Client
                        {
                            ID = sub.ID,
                            Name = sub.Name,
                        };
                        Client.Add(client);
                    }
                    HomeViewModel homeViewModel = new HomeViewModel
                    {
                        Userdata = userdata,
                        Clients = Client,
                    };
                    return View("Index", homeViewModel);
                }
                // client
                else
                {
                    HomeViewModel homeViewModel = new HomeViewModel
                    {
                        Userdata = userdata,
                    };
                    return View("Index", homeViewModel);
                }
            }
            else
            {
                return View();
            }
            
        }
        public ActionResult Dashboard()
        {
            if (Request.IsAuthenticated)
            {
                FormsIdentity user = (FormsIdentity)User.Identity;
                var struserdata = user.Ticket.UserData;
                var userdata = AccountController.UserDatastr(struserdata);
                if (userdata.AccessLevel == 2)
                {
                    return RedirectToAction("Index", new { id = User.Identity.Name });
                }
                else
                {
                    return View();
                }
            }
            else
            {
                return View();
            }
        }
    }
    
}