namespace NitStore.Models.DTO
{
    public class CampaignItemShowDTO
    {
        public int CampaignItemID { get; set; }
        public int CampaignID { get; set; }
        public int ProductId { get; set; }
        public string Name { get; set; }

        public int CategoryId { get; set; }

        public decimal Price { get; set; }

        public int Discount { get; set; }

        public byte[] imageBit { get; set; }
    }
}
