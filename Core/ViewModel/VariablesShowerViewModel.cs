using Datalayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ViewModel
{
    public class VariablesShowerViewModel
    {
        public List<VariationViewModel> AllVariations { get; set; }

        public VariationViewModel SelectedVariable { get; set; }

        public Product Product { get; set; }
    }
}
