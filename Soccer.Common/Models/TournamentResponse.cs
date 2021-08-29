using System;
using System.Collections.Generic;

namespace Soccer.Common.Models
{
    public class TournamentResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime StartDateLocal => StartDate.ToLocalTime();
        public DateTime EndDate { get; set; }
        public DateTime EndDateLocal => EndDate.ToLocalTime();
        public bool IsActive { get; set; }
        public string LogoPath { get; set; }

        public string LogoFullPath => string.IsNullOrEmpty(LogoPath)
            ? "https://soccer-web.conveyor.cloud//images/noimage.png"
            : $"https://soccer-web.conveyor.cloud/{LogoPath}";

        public List<GroupResponse> Groups { get; set; }
    }
}
