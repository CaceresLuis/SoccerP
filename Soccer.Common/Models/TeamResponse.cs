namespace Soccer.Common.Models
{
    public class TeamResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LogoPath { get; set; }
        public string LogoFullPath => string.IsNullOrEmpty(LogoPath)
            ? "https://soccer-web.conveyor.cloud/images/noimage.png"
            : $"https://soccer-web.conveyor.cloud/{LogoPath}";
    }
}
