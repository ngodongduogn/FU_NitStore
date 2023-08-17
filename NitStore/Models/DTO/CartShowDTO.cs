namespace NitStore.Models.DTO
{
    public class CartShowDTO
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        public byte[] imageBit { get; set; }

        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public string ProductDescription { get; set; }

        public string ProductCategory { get; set; }

        public decimal ProductPrice { get; set; }

        public int Quantity { get; set; }
    }
}
