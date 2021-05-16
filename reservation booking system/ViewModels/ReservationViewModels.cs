using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace reservation_booking_system.ViewModels
{ 
    public class ReservationdataView
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Start { get; set; }
        public string End { get; set; }
        public string ClassName { get; set; }
        public string Description { get; set; }
       

        [JsonProperty("extendedProps")]
        public  ExtendedProps ExtendedProps { get; set; }
    }

    public class ExtendedProps
    {
        [JsonProperty("ClientData")]
        public string ClientData { get; set; }
        [JsonProperty("Adminid")]
        public int Adminid { get; set; }
        [JsonProperty("ClientName")]
        public string ClientName { get; set; }
       
    } 
    
}