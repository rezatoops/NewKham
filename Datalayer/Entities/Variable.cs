using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datalayer.Entities
{
    public class Variable
    {
        [Key]
        public int Id { get; set; }

        public int ProductId { get; set; }

        [NotMapped]
        public string? Title
        {
            get
            {
                return string.Join(" - ", ProductVariableAttributes.Select(x => x.ProductAttributeValue.Value));
            }
        }

        public int? Price { get; set; }
        public int? SalePrice { get; set; }

        public int? NumberInStock { get; set; }

        public string? Description { get; set; }

        public Product Product { get; set; }

        public List<OrderProduct> OrderProducts { get; set; }
        public ICollection<ProductVariableAttribute> ProductVariableAttributes { get; set; }

        [NotMapped]
        public int FinalPrice
        {
            get
            {
                if (SalePrice != null)
                    return SalePrice.Value;
                else
                    return Price.Value;
            }
        }
    }
}
