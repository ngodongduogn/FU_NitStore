namespace NitStore.Models.DTO
{
    public class FeedbackOrderDTO
    {
        public int OrderID { get; set; }

        public int Rate { get; set; }
        public int ProductId { get; set; }

        public int CustomerId { get; set; }

        public string CustomerName { get; set; }
        public string ProductName { get; set; }

        public byte[] imageBit { get; set; }
        public string feedback { get; set; }
        public string DateFeedback { get; set; }
    }
}
