using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreCodeCamp.Models
{
    public class TalkModel
    {
        [Required]
        public int TalkId { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Title { get; set; }
        public string Abstract { get; set; }
        [Range(100, 500)]
        public int Level { get; set; }
        public SpeakModel Speaker { get; set; }
    }
}
