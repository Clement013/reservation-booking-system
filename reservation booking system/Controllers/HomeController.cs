using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using reservation_booking_system.Entity_Framework;
using reservation_booking_system.ViewModels;

namespace reservation_booking_system.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ReservationSystemDBEntities reservationSystemDBEntities = new ReservationSystemDBEntities();

            //var user = new Admin()
            //{
            //    Name = "Jakson"
            //};
            var addmmin = reservationSystemDBEntities.Admins.Where(x=>x.Status == 1).ToList();
            //var admin = new List<Admin>
            //{
            //    new Admin { Name = "Admin 1", ID = 1001 },
            //    new Admin { Name = "Admin 2", ID = 1002 }
            //};

            var clients = new List<Client>
            {
                new Client { Name = "Customer 1", ID = 1001 },
                new Client { Name = "Customer 2", ID = 1002 }
            };

            var viewModel = new UserAdminViewModel
            {
                Admins = addmmin,
                //Admin = addmmin,
                Clients = clients

            };

            return View(viewModel);
        }
        
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        
    }
}