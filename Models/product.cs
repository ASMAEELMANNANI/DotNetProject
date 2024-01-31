using ProductsCRUDMVC.Data.Enum;
using System.ComponentModel.DataAnnotations;

namespace ProductsCRUDMVC.Models
{
    public class product
    {
        [Key]
        public int IdProd { get; set; }
        public String Name { get; set; }
        public String Description { get; set; }
        public category Category { get; set; }
        public String Stock { get; set; }
        public String Image { get; set; }
        public float Price { get; set; }

    }
}
