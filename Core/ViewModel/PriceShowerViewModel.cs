using Datalayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ViewModel
{
    public class PriceShowerViewModel
    {
        public Variable? Variable { get; set; }

        public Product Product { get; set; }

        public int CurrentNumber { get; set; }
    }
}
