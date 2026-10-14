namespace HumoVolveDay2.DTOs
{
    public class CreateProductDto
    {
        public string Name { get; set; } = default!;
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
    }
}
