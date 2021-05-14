using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using reservation_booking_system.Entity_Framework;
using reservation_booking_system.ViewModels;
using Newtonsoft.Json;
using System.Web.Security;

namespace reservation_booking_system.Controllers
{
    public class ReservationController : Controller
    {
        [HttpPost]
        [Authorize]
        public JsonResult CreateReservdata()
        {
            FormsIdentity user = (FormsIdentity)User.Identity;
            var struserdata = user.Ticket.UserData;
            var userdata = AccountController.UserDatastr(struserdata);
            if (userdata.Role == "admin")
            {
                ReservationSystemDBEntities reservationSystemDBEntities = new ReservationSystemDBEntities();
                var data = Request.Form["reservationdata"].ToString();
                ReservationdataView reservationdata = JsonConvert.DeserializeObject<ReservationdataView>(data);
                var apr = Aprroval(reservationdata.ClassName);
                Event events = new Event
                {
                    Title = reservationdata.Title,
                    FromTime = reservationdata.Start,
                    EndTime = reservationdata.End,
                    Description = reservationdata.Description,
                    Approval = apr,
                    AdminID = Int32.Parse(User.Identity.Name),
                    EventID = reservationdata.Id,
                    ClientID = Int32.Parse(reservationdata.ExtendedProps.ClientData),
                    CreatedBy = Int32.Parse(User.Identity.Name),
                    UpdatedBy = Int32.Parse(User.Identity.Name),
                    CreatedTime = DateTime.Now.ToString(),
                    UpdatedTime = DateTime.Now.ToString(),
                    Status = 1,
                };
                reservationSystemDBEntities.Events.Add(events);
                reservationSystemDBEntities.SaveChanges();
                reservationdata.ExtendedProps.AdminData = User.Identity.Name;
                var username = reservationSystemDBEntities.Clients.Where(x => x.ID == events.ClientID).Select(x=>x.UserName).FirstOrDefault();
                reservationdata.Description = reservationdata.Description + ". Booking by " + username;
                return Json(new { success = "admin", text = "Add data success", reservationdata});
            }
            else if (userdata.Role == "client")
            {

                return Json(new { success = "client", text = "ajax sucess" });
            }
            else
            {
                return Json(new { success = false, text = "you don't have permission to access" });
            }

        }
        [HttpPost]
        public JsonResult GetRsrData(string urlid)
        {
            ReservationSystemDBEntities reservationSystemDBEntities = new ReservationSystemDBEntities();
            List<ReservationdataView> Reservationdt = new List<ReservationdataView>();
            var urlidint = Int32.Parse(urlid);
            var dbdata = reservationSystemDBEntities.Events.Where(x => x.Status == 1 && x.AdminID == urlidint).ToList();
            if(!(dbdata == null)){
                UserData userdata = new UserData();
                if (Request.IsAuthenticated)
                {
                    FormsIdentity user = (FormsIdentity)User.Identity;
                    var struserdata = user.Ticket.UserData;
                    userdata = AccountController.UserDatastr(struserdata);
                }
                // role ==  admin
                if (userdata.Role == "admin")
                    {
                        foreach (var sub in dbdata)
                        {
                            var rtappr = ReturnAppr(sub.Approval);
                            var extendedProps = new ExtendedProps
                            {
                                ClientData = sub.ClientID.ToString(),
                                AdminData = sub.AdminID.ToString()

                            };
                            var reservationdataView = new ReservationdataView
                            {
                                Id = sub.EventID,
                                Title = sub.Title,
                                Start = sub.FromTime,
                                End = sub.EndTime,
                                ClassName = rtappr,
                                Description = sub.Description + ". Booking by " + sub.Client.UserName,
                                ExtendedProps = extendedProps
                            };
                            Reservationdt.Add(reservationdataView);
                        }
                        return Json(new { success = true, Reservationdt }, JsonRequestBehavior.AllowGet);
                    }
                
                // not sign in and role client
                else
                {
                    foreach (var sub in dbdata)
                    {
                        var rtappr = ReturnAppr(sub.Approval);
                        var extendedProps = new ExtendedProps
                        {
                            ClientData = "",
                            AdminData = ""

                        };
                        var reservationdataView = new ReservationdataView
                        {
                            Id = "",
                            Title = "Reservation",
                            Start = sub.FromTime,
                            End = sub.EndTime,
                            ClassName = rtappr,
                            Description = "",
                            ExtendedProps = extendedProps
                        };
                        Reservationdt.Add(reservationdataView);
                    }
                    return Json(new { success = true, Reservationdt }, JsonRequestBehavior.AllowGet);
                }
            }
            // no data
            else
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        public static string ReturnAppr(string appr)
        {
            if (appr == "Approval")
            {
                return "bg-success";
            }
            else
            {
                return "bg-danger";
            }
        }
        public static string Aprroval(string appr)
        {
            if(appr == "bg-success")
            {
                return "Approval";
            }
            else
            {
                return "Unapproval";
            }
        }
        
    }
}
