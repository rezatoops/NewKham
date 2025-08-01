using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datalayer.Entities
{
    public class Spec
    {
        [Key]
        public int Id { get; set; }

        public int ProductId { get; set; }

        [MaxLength(300)]
        public string SpecKey { get; set; }

        [MaxLength(300)]
        public string? SpecValue { get; set; }
    }
}
