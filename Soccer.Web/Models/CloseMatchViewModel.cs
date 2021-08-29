using Soccer.Web.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace Soccer.Web.Models
{
    public class CloseMatchViewModel
    {
        public int MatchId { get; set; }

        public int GroupId { get; set; }

        public int LocalId { get; set; }

        public int VisitorId { get; set; }

        [Display(Name = "Goals Local")]
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [Range(0, 20,
        ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public int? GoalsLocal { get; set; }

        [Display(Name = "Goals Visitor")]
        [Range(0, 20,
        ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        public int? GoalsVisitor { get; set; }

        public GroupEntity Group { get; set; }

        public TeamEntity Local { get; set; }

        public TeamEntity Visitor { get; set; }
    }
}