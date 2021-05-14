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
        
    }
}