using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using reservation_booking_system.Entity_Framework;
using reservation_booking_system.ViewModels;
using Newtonsoft.Json;
using System.Web.Security;
using reservation_booking_system.Mail;
using System.IO;

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
                    AdminID = reservationdata.ExtendedProps.Adminid,
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
                
                return Json(new { success = "admin", text = "Add data success"});
            }
            else if (userdata.Role == "client")
            {
                ReservationSystemDBEntities reservationSystemDBEntities = new ReservationSystemDBEntities();
                var data = Request.Form["reservationdata"].ToString();
                ReservationdataView reservationdata = JsonConvert.DeserializeObject<ReservationdataView>(data);
                var apr = Aprroval(reservationdata.ClassName);
                var clientdt = reservationSystemDBEntities.Clients.Where(x => x.UserName == reservationdata.ExtendedProps.ClientData).FirstOrDefault();

                Event events = new Event
                {
                    Title = reservationdata.Title,
                    FromTime = reservationdata.Start,
                    EndTime = reservationdata.End,
                    Description = reservationdata.Description,
                    Approval = apr,

                    AdminID = reservationdata.ExtendedProps.Adminid,
                    EventID = reservationdata.Id,
                    ClientID = clientdt.ID,
                    CreatedBy = clientdt.ID,
                    UpdatedBy = clientdt.ID,
                    CreatedTime = DateTime.Now.ToString(),
                    UpdatedTime = DateTime.Now.ToString(),
                    Status = 1,
                };
                reservationSystemDBEntities.Events.Add(events);
                var a = reservationSystemDBEntities.SaveChanges();

                // send email to client
                var eventdata = reservationSystemDBEntities.Events.Where(x => x.EventID == events.EventID && x.AdminID == events.AdminID).FirstOrDefault();
                var admindata = reservationSystemDBEntities.Admins.Where(x => x.ID == eventdata.AdminID).FirstOrDefault();
                MailServer.ReservationsendClientEmail(admindata, eventdata);
                MailServer.ReservationsendadminEmail(admindata, eventdata);
                return Json(new { success = "client", text = "Reserve sucess" });
            }
            else
            {
                return Json(new { success = false, text = "you don't have permission to access" });
            }

        }

        // get the reservation for each admin by ajax  // index page
        [HttpPost]
        public JsonResult GetRsrData(int urlid)
        {
            ReservationSystemDBEntities reservationSystemDBEntities = new ReservationSystemDBEntities();
            List<ReservationdataView> Reservationdt = new List<ReservationdataView>();
            var dbdata = reservationSystemDBEntities.Events.Where(x => x.Status == 1 && x.AdminID == urlid).ToList();
            if(!(dbdata.Count == 0)){
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
                        var alldbdata = reservationSystemDBEntities.Events.Where(x => x.AdminID == urlid).ToList();
                        foreach (var sub in alldbdata)
                        {
                            var rtappr = ReturnAppr(sub.Approval);
                            var extendedProps = new ExtendedProps
                            {
                                ClientData = sub.ClientID.ToString(),
                                ClientName = sub.Client.UserName
                            };
                            var reservationdataView = new ReservationdataView
                            {
                                Id = sub.EventID,
                                Title = sub.Title,
                                Start = sub.FromTime,
                                End = sub.EndTime,
                                ClassName = rtappr,
                                Description = sub.Description,
                                ExtendedProps = extendedProps
                            };
                            Reservationdt.Add(reservationdataView);
                        }
                        return Json(new { success = true, Reservationdt }, JsonRequestBehavior.AllowGet);
                    }
                // role client
                else if (userdata.Role == "client")
                {
                    var clientdt = reservationSystemDBEntities.Clients.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();
                    
                    foreach (var sub in dbdata)
                    {
                        DateTime timestamp = DateTime.ParseExact(sub.EndTime, "yyyy-MM-ddTHH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                        if (DateTime.Now < timestamp)
                        {
                            var rtappr = ReturnAppr(sub.Approval);
                            if (clientdt.ID == sub.ClientID)
                            {
                                var extendedProps = new ExtendedProps
                                {
                                    ClientData = sub.ClientID.ToString(),
                                };
                                var reservationdataView = new ReservationdataView
                                {
                                    Id = sub.EventID,
                                    Title = sub.Title,
                                    Start = sub.FromTime,
                                    End = sub.EndTime,
                                    ClassName = rtappr,
                                    Description = sub.Description,
                                    ExtendedProps = extendedProps
                                };
                                Reservationdt.Add(reservationdataView);
                            }
                            else
                            {
                                var extendedProps = new ExtendedProps
                                {
                                    ClientData = ""
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
                        }
                        
                    }
                    return Json(new { success = true, Reservationdt }, JsonRequestBehavior.AllowGet);
                }
                // not sign in
                else
                {

                    foreach (var sub in dbdata)
                    {
                        DateTime timestamp = DateTime.ParseExact(sub.EndTime, "yyyy-MM-ddTHH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                        if (DateTime.Now < timestamp)
                        {
                            
                            var rtappr = ReturnAppr(sub.Approval);
                            var extendedProps = new ExtendedProps
                            {
                                ClientData = ""
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

        [HttpPost]
        [Authorize]
        public JsonResult UpdateRserv()
        {
            FormsIdentity user = (FormsIdentity)User.Identity;
            var struserdata = user.Ticket.UserData;
            var userdata = AccountController.UserDatastr(struserdata);
            if (userdata.Role == "admin")
            {
                ReservationSystemDBEntities reservationSystemDBEntities = new ReservationSystemDBEntities();
                var data = Request.Form["reservationdata"].ToString();
                ReservationdataView reservationdata = JsonConvert.DeserializeObject<ReservationdataView>(data);
                var strid = Int32.Parse(User.Identity.Name);
                var apr = Aprroval(reservationdata.ClassName);

                var eventdt = reservationSystemDBEntities.Events.Where(x => x.EventID == reservationdata.Id && x.AdminID == strid).FirstOrDefault();
                var defaultapproval = eventdt.Approval;
                eventdt.Title = reservationdata.Title;
                eventdt.FromTime = reservationdata.Start;
                eventdt.EndTime = reservationdata.End;
                eventdt.Description = reservationdata.Description;
                eventdt.Approval = apr;
                eventdt.UpdatedBy = strid;
                eventdt.UpdatedTime = DateTime.Now.ToString();
                eventdt.Status = 1;
                reservationSystemDBEntities.SaveChanges();
                
                if (apr== "Approved" && defaultapproval == "Pending")
                {
                    var eventdata = reservationSystemDBEntities.Events.Where(x => x.EventID == reservationdata.Id && x.AdminID == strid && x.Status == 1).FirstOrDefault();
                    var admindata = reservationSystemDBEntities.Admins.Where(x => x.ID == eventdata.AdminID).FirstOrDefault();
                    MailServer.AproveReservationsendEmail(admindata,eventdata);
                }
                
                return Json(new { success = true, text = "Update Reservation Sucess" });

            }
            else
            {
                return Json(new { success = false, text = "you don't have permission to access" });
            }

        }
        [HttpPost]
        [Authorize]
        public JsonResult DeleteRsv()
        {
            FormsIdentity user = (FormsIdentity)User.Identity;
            var struserdata = user.Ticket.UserData;
            var userdata = AccountController.UserDatastr(struserdata);
            if (userdata.Role == "admin")
            {
                ReservationSystemDBEntities reservationSystemDBEntities = new ReservationSystemDBEntities();
                var data = Request.Form["deletereservation"].ToString();
                ReservationdataView reservationdata = JsonConvert.DeserializeObject<ReservationdataView>(data);
                var strid = Int32.Parse(User.Identity.Name);
                
                var eventdt = reservationSystemDBEntities.Events.Where(x => x.EventID == reservationdata.Id && x.AdminID == strid).FirstOrDefault();
                
                // set status = 0
                var defaultapproval = eventdt.Approval;
                eventdt.Status = 0;
                eventdt.UpdatedBy = strid;
                eventdt.UpdatedTime = DateTime.Now.ToString();
                eventdt.Approval = "Rejected";
                reservationSystemDBEntities.SaveChanges();

                if (defaultapproval == "Pending")
                {
                    var eventdata = reservationSystemDBEntities.Events.Where(x => x.EventID == reservationdata.Id && x.AdminID == strid && x.Status == 0).FirstOrDefault();
                    var admindata = reservationSystemDBEntities.Admins.Where(x => x.ID == eventdata.AdminID).FirstOrDefault();
                    MailServer.CancelReservationsendEmail(admindata, eventdata);
                }

                return Json(new { success = true, text = "Delete Reservation Sucess" });

            }
            else
            {
                return Json(new { success = false, text = "you don't have permission to access" });
            }

        }
        public static string ReturnAppr(string appr)
        {
            if (appr == "Approved")
            {
                return "bg-success";
            }
            else if (appr == "Pending")
            {
                return "bg-primary";
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
                return "Approved";
            }
            else if(appr == "bg-danger")
            {
                return "Rejected";
            }
            else
            {
                return "Pending";
            }
        }
        
    }
}
