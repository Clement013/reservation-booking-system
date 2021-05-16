using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using reservation_booking_system.Entity_Framework;

namespace reservation_booking_system.ViewModels
{
    
    public class HomeViewModel
    {
        public UserData Userdata { get; set; }

        public List<Client> Clients { get; set; }
        public List<DashboardviewModel> Dashview { get; set; }
        
    }
    public class DashboardviewModel
    {
        public string Name { get; set; }

        public string Email { get; set; }
        public int Id { get; set; }
        public int Reservation { get; set; }
    }
    
}