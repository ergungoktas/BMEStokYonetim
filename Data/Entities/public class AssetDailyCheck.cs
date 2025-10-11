namespace BMEStokYonetim.Data.Entities
{
    public class AssetResponsibility
    {
        public int Id { get; set; }

        public int AssetId { get; set; }
        public Asset Asset { get; set; } = default!;

        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = default!;

        public DateTime AssignedDate { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;
    }
}
