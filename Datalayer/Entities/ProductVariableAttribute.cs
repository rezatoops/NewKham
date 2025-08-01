using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datalayer.Entities
{
    public class ProductVariableAttribute
    {
        public int Id { get; set; }
        public int VariableId { get; set; }

        public int? ProductAttributeId { get; set; }
        public int? ProductAttributeValueId { get; set; }

        public int? ProductId { get; set; }

        public Product Product { get; set; }
        public Variable Variable { get; set; }
        public ProductAttribute ProductAttribute { get; set; }
        public ProductAttributeValue ProductAttributeValue { get; set; }
    }
}
