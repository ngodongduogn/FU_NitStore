namespace NitStore.Models.DTO
{
    public class SliderShowDTO
    {
        public int SliderId { get; set; }
        public int CampaignId { get; set; }
        public string CampaignName { get; set; }
        public byte[] ImageData { get; set; }
        public bool Status { get; set; }
    }
}
