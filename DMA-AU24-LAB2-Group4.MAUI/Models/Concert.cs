using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMA_AU24_LAB2_Group4.MAUI.Models
{
    public class Concert
    {
        public int Id { get; set; } = 0!;
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        
        /// <summary>
        /// URL to the artist/concert promotional image
        /// </summary>
        public string? ImageUrl { get; set; }
        
        //Navigation properties
        public ICollection<Performance>? Performances { get; set; }
    }
}
