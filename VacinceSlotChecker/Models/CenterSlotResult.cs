using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaccinceSlotChecker.Models
{
    public class CenterSlotResult
    {
        public string center_id { get; set; }
        public string CTC_NATURE { get; set; }
        public List<TimeSlotDetail> avalible_timeslots { get; set; }
        public List<TimeSlot> freeTimeslots { get; set; }
    }

    public class TimeSlotDetail
    {
        public DateTime date { get; set; }
        public List<TimeSlot> timeslots { get; set; }
    }

    public class TimeSlot
    {
        public string timeslots_id { get; set; }
        public string time { get; set; }
        public string display_label { get; set; }
        public DateTime datetime { get; set; }
        public int value { get; set; }
    }

    public class CenterTimeSlot : TimeSlot 
    {   
        public string center_id { get; set; }
    }
}
