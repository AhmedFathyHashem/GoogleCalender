using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace GoogleCalender.Models
{
    public class SetCalenderEvent
    {
        [Required]
        public string Summary { get; set; }
        [Required]
        public string Description { get; set; }
        public string Location { get; set; }
        [Required]
        public DateTime Start { get; set; }
        [Required]
        public DateTime End { get; set; }
    }
}
