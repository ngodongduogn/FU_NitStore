namespace NitStore.Models.DTO
{
    public class OrderHistory
    {
        public int Id { get; set; }

        public int Quantity { get; set; }
        public string OrderDate { get; set; }

        public string Status { get; set; }

        public decimal TotalMoney { get; set; }

        public bool FeedbackAble { get; set; }
    }
}
