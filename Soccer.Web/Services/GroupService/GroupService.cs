using Microsoft.EntityFrameworkCore;
using Soccer.Web.Data;
using Soccer.Web.Data.Entities;
using Soccer.Web.Helpers;
using Soccer.Web.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Soccer.Web.Services.GroupService
{
    public class GroupService : IGroupService
    {
        private readonly DataContext _context;
        private readonly ICombosHelper _combosHelper;

        public GroupService(
            DataContext context,
            ICombosHelper combosHelper)
        {
            _context = context;
            _combosHelper = combosHelper;
        }

        public async Task<GroupEntity> AddGroupAsync(GroupEntity group)
        {
            _context.Add(group);
            await _context.SaveChangesAsync();
            return (group);
        }

        public async Task<GroupEntity> EditGroupAsync(GroupEntity group)
        {
            _context.Update(group);
            await _context.SaveChangesAsync();
            return (group);
        }

        public async Task<GroupEntity> GetFindGroupsAsync(int id)
        {
            return await _context.Groups.FindAsync(id);
        }

        public async Task<GroupEntity> GetGroupTournamentsAsync(int id)
        {
            return await _context.Groups
                .Include(g => g.Tournament)
                .FirstOrDefaultAsync(g => g.Id == id);
        }

        public async Task<GroupEntity> GetFullGroupAsync(int id)
        {
            return await _context.Groups
                .Include(g => g.Matches)
                .ThenInclude(g => g.Local)
                .Include(g => g.Matches)
                .ThenInclude(g => g.Visitor)
                .Include(g => g.Tournament)
                .Include(g => g.GroupDetails)
                .ThenInclude(gd => gd.Team)
                .FirstOrDefaultAsync(g => g.Id == id);
        }

        public string ComprobateDelete(int idGroup)
        {
            try
            {
                var groupDetails = "";
                var group = _context.GroupDetails
                    .FirstOrDefault(g => g.Group.Id == idGroup);
                if (group != null)
                {
                    groupDetails = "Este grupo tiene equipos";
                    return (groupDetails);
                }
            }
            catch (System.Exception)
            {
                return "OK";
            }
            return "OK";
        }

        public async Task<GroupEntity> DeleteGroupAsync(GroupEntity group)
        {
            _context.Remove(group);
            await _context.SaveChangesAsync();
            return group;
        }


        public async Task<GroupEntity> ToGroupEntityAsync(GroupViewModel model, bool isNew)
        {
            return new GroupEntity
            {
                GroupDetails = model.GroupDetails,
                Id = isNew ? 0 : model.Id,
                Matches = model.Matches,
                Name = model.Name,
                Tournament = await _context.Tournaments.FindAsync(model.TournamentId)
            };
        }

        public GroupViewModel ToGroupViewModel(GroupEntity groupEntity)
        {
            return new GroupViewModel
            {
                GroupDetails = groupEntity.GroupDetails,
                Id = groupEntity.Id,
                Matches = groupEntity.Matches,
                Name = groupEntity.Name,
                Tournament = groupEntity.Tournament,
                TournamentId = groupEntity.Tournament.Id
            };
        }

        public async Task<TournamentEntity> GetTournamentFindAsync(int id)
        {
            return await _context.Tournaments.FindAsync(id);
        }

    }
}
