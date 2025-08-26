using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HolidayHome.Models
{
    [Table("HolidayMaster")]
    public class HolidayMaster
    {
        [Key]
        public int HolidayMasterId { get; set; }

        [StringLength(100)]
        public string? Location { get; set; }

        [StringLength(255)]
        public string? Details { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? PricePerDay { get; set; }
    }
}
