using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arushanyan_LB4.Data
{
    [Table("Users")]
    public class User
    {
        [Key]
        public int ID { get; set; }
        [Required, StringLength(50)]
        public string First_Name { get; set; }
        [Required, StringLength(50)]
        public string Last_Name { get; set; }
        public int Role_ID { get; set; }
        [ForeignKey("Role_ID")]
        public virtual Role Role { get; set; }
    }
}
