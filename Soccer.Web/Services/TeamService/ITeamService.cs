using Soccer.Web.Data.Entities;
using Soccer.Web.Models;
using System.Threading.Tasks;

namespace Soccer.Web.Services.TeamService
{
    public interface ITeamService
    {
        Task<TeamEntity[]> GetTeamList();
        Task<TeamEntity> GetFindEntity(int id);
        Task<TeamEntity> AddTeamAsync(TeamEntity team);
        Task<TeamEntity> EditTeamAsync(TeamEntity team);
        Task<TeamEntity> DeleteTeamAsync(TeamEntity team);

        TeamEntity ToTeamEntity(TeamViewModel model, string path, bool isNew);
        TeamViewModel ToTeamViewModel(TeamEntity teamEntity);
        string ComprobateDelete(int idTeam);
        Task<TeamEntity> AddOrUpdateTeamAsync(TeamEntity teamEntity, bool isNew);
    }
}