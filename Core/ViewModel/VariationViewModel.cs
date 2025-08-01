using Datalayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ViewModel
{
    public class VariationViewModel
    {
        public int Id { get; set; }
        public int Price { get; set; }
        public int? SalePrice { get; set; }
        public int NumberInStock { get; set; }
        public int? VariableId { get; set; }
        public Product Product { get; set; }
        public List<AttributeViewModel> Attributes { get; set; }
    }

    public class AttributeViewModel
    {
        public int? AttributeId { get; set; }
        public string AttributeName { get; set; }

        public int? ValueId { get; set; }
        public string ValueName { get; set; }
    }
}
