using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace HumoVolveDay2.Models
{
    public class Product
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        [Precision(18, 2)]
        public decimal Price { get; set; }

        public int CategoryId { get; set; }

        public Category Category { get; set; } = null!;

        public bool IsDeleted { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; } = null!;
    }
}
