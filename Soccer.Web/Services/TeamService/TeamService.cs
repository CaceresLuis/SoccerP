using Microsoft.EntityFrameworkCore;
using Soccer.Web.Data;
using Soccer.Web.Data.Entities;
using Soccer.Web.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Soccer.Web.Services.TeamService
{
    public class TeamService : ITeamService
    {
        private readonly DataContext _context;

        public TeamService(DataContext context)
        {
            _context = context;
        }

        public async Task<TeamEntity[]> GetTeamList()
        {
            return await _context.Teams.ToArrayAsync();
        }

        public async Task<TeamEntity> GetFindEntity(int id)
        {
            return await _context.Teams.FindAsync(id);
        }

        public async Task<TeamEntity> AddTeamAsync(TeamEntity teamEntity)
        {
            _context.Add(teamEntity);
            await _context.SaveChangesAsync();
            return (teamEntity);
        }

        public async Task<TeamEntity> EditTeamAsync(TeamEntity teamEntity)
        {
            _context.Update(teamEntity);
            await _context.SaveChangesAsync();
            return (teamEntity);
        }

        public async Task<TeamEntity> AddOrUpdateTeamAsync(TeamEntity teamEntity, bool isNew)
        {
            var addOrUdate = isNew ? _context.Add(teamEntity) : _context.Update(teamEntity);
            await _context.SaveChangesAsync();
            return (teamEntity);
        }

        public string ComprobateDelete(int idTeam)
        {
            try
            {
                var team = "";
                var group = "";
                var userTeam = _context.Users
                        .FirstOrDefault(u => u.Team.Id == idTeam);
                if (userTeam != null)
                {
                    team = "Este equipo es favorito de algun usuario";
                    return (team);
                }

                var groupDetil = _context.GroupDetails
                    .FirstOrDefault(g => g.Team.Id == idTeam);
                if (groupDetil != null)
                {
                    group = "Este equipo esta en un torneo";
                    return (group);
                }
            }
            catch (System.Exception)
            {
                return "OK";
            }
            return "OK";
        }

        public async Task<TeamEntity> DeleteTeamAsync(TeamEntity teamEntity)
        {
            _context.Teams.Remove(teamEntity);
            await _context.SaveChangesAsync();
            return (teamEntity);
        }

        public TeamEntity ToTeamEntity(TeamViewModel model, string path, bool isNew)
        {
            return new TeamEntity
            {
                Id = isNew ? 0 : model.Id,
                LogoPath = path,
                Name = model.Name
            };
        }

        public TeamViewModel ToTeamViewModel(TeamEntity teamEntity)
        {
            return new TeamViewModel
            {
                Id = teamEntity.Id,
                LogoPath = teamEntity.LogoPath,
                Name = teamEntity.Name
            };
        }

    }
}
