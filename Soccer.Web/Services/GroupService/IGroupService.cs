using Soccer.Web.Data.Entities;
using Soccer.Web.Models;
using System.Threading.Tasks;

namespace Soccer.Web.Services.GroupService
{
    public interface IGroupService
    {
        Task<GroupEntity> GetFullGroupAsync(int id);
        Task<GroupEntity> GetFindGroupsAsync(int id);
        Task<GroupEntity> GetGroupTournamentsAsync(int id);
        Task<GroupEntity> AddGroupAsync(GroupEntity group);
        Task<GroupEntity> EditGroupAsync(GroupEntity group);
        Task<GroupEntity> DeleteGroupAsync(GroupEntity group);

        Task<GroupEntity> ToGroupEntityAsync(GroupViewModel model, bool isNew);
        GroupViewModel ToGroupViewModel(GroupEntity groupEntity);

        Task<TournamentEntity> GetTournamentFindAsync(int id);
        string ComprobateDelete(int idGroup);
    }
}
