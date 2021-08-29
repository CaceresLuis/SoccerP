using Microsoft.EntityFrameworkCore;
using Soccer.Web.Data;
using Soccer.Web.Data.Entities;
using Soccer.Web.Helpers;
using Soccer.Web.Models;
using System.Threading.Tasks;

namespace Soccer.Web.Services.GroupDetail
{
    public class GroupDetailService : IGroupDetailService
    {
        private readonly DataContext _context;
        private readonly ICombosHelper _combosHelper;

        public GroupDetailService(
            DataContext context,
            ICombosHelper combosHelper)
        {
            _context = context;
            _combosHelper = combosHelper;
        }

        public async Task<GroupDetailEntity> GetGroupDetailsAsync(int id)
        {
            return await _context.GroupDetails
                .Include(gd => gd.Group)
                .Include(gd => gd.Team)
                .FirstOrDefaultAsync(gd => gd.Id == id);
        }

        public async Task<GroupDetailEntity> AddOrUpdateGroupDetailsAsync(GroupDetailEntity groupDetail, bool isNew)
        {
            if(isNew == true)
            {
                _context.Add(groupDetail);
                await _context.SaveChangesAsync();
                return (groupDetail);
            }

            _context.Update(groupDetail);
            await _context.SaveChangesAsync();
            return (groupDetail);
        }
        
        //public async Task<GroupDetailEntity> EditGroupDetailsAsync(GroupDetailEntity groupDetail)
        //{
        //    _context.Update(groupDetail);
        //    await _context.SaveChangesAsync();
        //    return (groupDetail);
        //}

        //public async Task<GroupDetailEntity> AddGroupDetailAsync(GroupDetailEntity groupDetail)
        //{
        //    _context.Add(groupDetail);
        //    await _context.SaveChangesAsync();
        //    return (groupDetail);
        //}

        public async Task<GroupDetailEntity> DeleteGroupDetailsAsync(GroupDetailEntity groupDetail)
        {
            _context.Remove(groupDetail);
            await _context.SaveChangesAsync();
            return groupDetail;
        }

        public async Task<GroupEntity> GetFindGroupsAsync(int id)
        {
            return await _context.Groups.FindAsync(id);
        }

        public async Task<GroupDetailEntity> ToGroupDetailEntityAsync(GroupDetailViewModel model, bool isNew)
        {
            return new GroupDetailEntity
            {
                GoalsAgainst = model.GoalsAgainst,
                GoalsFor = model.GoalsFor,
                Group = await _context.Groups.FindAsync(model.GroupId),
                Id = isNew ? 0 : model.Id,
                MatchesLost = model.MatchesLost,
                MatchesPlayed = model.MatchesPlayed,
                MatchesTied = model.MatchesTied,
                MatchesWon = model.MatchesWon,
                Team = await _context.Teams.FindAsync(model.TeamId)
            };
        }

        public async Task<GroupDetailViewModel> ToGroupDetailViewModel(GroupDetailEntity groupDetailEntity)
        {
            return new GroupDetailViewModel
            {
                GoalsAgainst = groupDetailEntity.GoalsAgainst,
                GoalsFor = groupDetailEntity.GoalsFor,
                Group = groupDetailEntity.Group,
                GroupId = groupDetailEntity.Group.Id,
                Id = groupDetailEntity.Id,
                MatchesLost = groupDetailEntity.MatchesLost,
                MatchesPlayed = groupDetailEntity.MatchesPlayed,
                MatchesTied = groupDetailEntity.MatchesTied,
                MatchesWon = groupDetailEntity.MatchesWon,
                Team = groupDetailEntity.Team,
                TeamId = groupDetailEntity.Team.Id,
                Teams = await _context.Teams.FindAsync(groupDetailEntity.Team.Id)
            };
        }
    }
}
