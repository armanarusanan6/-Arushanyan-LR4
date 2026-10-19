using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arushanyan_LB4.Data
{
    [Table("Roles")]
    public class Role
    {
        [Key]
        public int ID { get; set; }
        [Required, StringLength(50)]
        public string Role_Name { get; set; }
        public virtual ICollection<User> Users { get; set; }
        public override string ToString() { return Role_Name; }
    }
}
