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
                    if(id.ToString() == User.Identity.Name)
                    {
                        ReservationSystemDBEntities reservationSystemDBEntities = new ReservationSystemDBEntities();
                        List<Client> Client = new List<Client>();
                        var clientdata = reservationSystemDBEntities.Clients.Where(x => x.Status == 1).ToList();
                        foreach (var sub in clientdata)
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
                        return View(homeViewModel);
                    }
                    else
                    {
                        return View("Error");
                    }
                    
                }
                // client
                else
                {
                    HomeViewModel homeViewModel = new HomeViewModel
                    {
                        Userdata = userdata,
                    };
                    return View(homeViewModel);
                }
            }
            else
            {
                return View();
            }
            
        }

        [Route("home/dashboard")]
        public ActionResult Dashboard()
        {
            UserData userdata = new UserData();
            if (Request.IsAuthenticated)
            {
                FormsIdentity user = (FormsIdentity)User.Identity;
                var struserdata = user.Ticket.UserData;
                userdata = AccountController.UserDatastr(struserdata);
            }
            
            if (userdata.AccessLevel == 2)
            {
                return RedirectToAction("Index", new { id = User.Identity.Name });
            }
            else
            {
                List<DashboardviewModel> Reservationdt = new List<DashboardviewModel>();
                ReservationSystemDBEntities reservationSystemDBEntities = new ReservationSystemDBEntities();
                var admindt = reservationSystemDBEntities.Admins.Where(x => x.Status == 1).Select(x => new { x.Name, x.ID, x.Email }).ToList();
                foreach (var sub in admindt)
                {
                    int datacount = reservationSystemDBEntities.Events.Where(x => x.AdminID == sub.ID && x.Status == 1 && x.Approval == "Approved").Count();
                    var dashdata = new DashboardviewModel
                    {
                        Name = sub.Name,
                        Email = sub.Email,
                        Id = sub.ID,
                        Reservation = datacount
                    };
                    Reservationdt.Add(dashdata);
                }
                HomeViewModel homeViewModel = new HomeViewModel
                {
                    Userdata = userdata,
                    Dashview = Reservationdt
                };
                
                return View(homeViewModel);
            }
           
        }
    }
    
}