using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datalayer.Entities
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(300)]
        public string? Title { get; set; }

        [MaxLength(300)]
        public string? EnglishTitle { get; set; }

        [MaxLength(2000)]
        public string? Slug { get; set; }

        public string? Review { get; set; }

        public int? PhotoId { get; set; }

        [MaxLength(100)]
        public string? Gallery { get; set; }

        public int UserId { get; set; }

        public DateTime CreateTime { get; set; }

        [MaxLength(50)]
        public string? Status { get; set; }

        public bool IsWonderProduct { get; set; }

        public int NumberSale { get; set; }

        public DateTime? WonderTime { get; set; }

        [MaxLength(3000)]
        public string? MetaDescription { get; set; }

        public bool IsDeleted { get; set; }
        public bool IsHidden { get; set; }

        public bool IsSimple { get; set; }
        public int? BasePrice { get; set; }
        public int? BaseSalePrice { get; set; }

        public int? BaseStockNumber { get; set; }

        [MaxLength(500)]
        public string? BrandName { get; set; }
        [MaxLength(500)]
        public string? CountSpec { get; set; }
        [MaxLength(500)]
        public string? WeightSpec { get; set; }
        [MaxLength(500)]
        public string? Spec1 { get; set; }
        [MaxLength(500)]
        public string? Spec2 { get; set; }
        [MaxLength(500)]
        public string? Spec3 { get; set; }

        [MaxLength(500)]
        public string? Mahsul { get; set; }

        [MaxLength(2000)]
        public string? ExtraDescription { get; set; }

        [MaxLength(20)]
        public string? WonderJobId { get; set; }

        public bool IsIsoHide { get; set; }

        //public List<ProductAttribute> ProductAttribute { get; set; }
        //public List<ProductCat> ProductCat { get; set; }


        //public List<ProductTag> ProductTag { get; set; }

        //public List<WonderProduct> WonderProduct { get; set; }

        public List<Comment> Comments { get; set; }

        //public List<Notification> Notifications { get; set; }

        public List<Variable> Variables { get; set; }

        public List<OrderProduct> OrderProducts { get; set; }
        public Media Photo { get; set; }
        public User User { get; set; }

        public ICollection<ProductAttribute> Attributes { get; set; }

        [NotMapped]
        public int FinalPrice
        {
            get
            {
                if (BaseSalePrice != null)
                    return BaseSalePrice.Value;
                else
                    return BasePrice.Value;
            }
        }

        [NotMapped]
        public bool IsInStock
        {
            get
            {
                if (Variables != null)
                {
                    return Variables.Sum(x => x.NumberInStock) > 0;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
