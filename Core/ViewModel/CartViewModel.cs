using Core.Enums;
using Datalayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ViewModel
{
    public class CartViewModel
    {
        public Order? Order { get; set; }


        public List<string> ErrorMessages { get; set; } = new List<string>();

        public CartStatus CartStatus { get; set; }
    }
}
