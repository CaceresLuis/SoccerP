using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Soccer.Web.Data.Entities
{
    public class TeamEntity
    {
        public int Id { get; set; }

        [MaxLength(100, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        public string Name { get; set; }

        [Display(Name = "Logo")]
        public string LogoPath { get; set; }
        public ICollection<GroupDetailEntity> GroupDetails { get; set; }
        public ICollection<UserEntity> Users { get; set; }

        [Display(Name = "Logo")]
        public string LogoFullPath => string.IsNullOrEmpty(LogoPath)
            ? "https://soccer-web.conveyor.cloud/images/noimage.png"
            : $"https://soccer-web.conveyor.cloud/{LogoPath.Substring(1)}";
    }
}
