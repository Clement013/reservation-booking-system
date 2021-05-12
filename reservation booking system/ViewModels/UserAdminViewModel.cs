using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using reservation_booking_system.Entity_Framework;

namespace reservation_booking_system.ViewModels
{
    public class UserAdminViewModel
    {
        public List<Admin> Admins { get; set; }
        public List<Client> Clients  { get; set; }
    }
}