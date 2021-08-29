using Soccer.Web.Data.Entities;
using Soccer.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Soccer.Web.Services.MatchService
{
    public interface IMatchService
    {
        Task<MatchEntity> GetMatchAsync(int id);
        Task<MatchEntity> GetMatchGroupAsync(int id);
        Task<MatchEntity> AddMatchAsync(MatchEntity match);
        Task EditMatchAsync(int matchId, int goalsLocal, int goalsVisitor);
        Task<MatchEntity> DeleteGroupAsync(MatchEntity match);
        MatchViewModel ToMatchViewModel(MatchEntity matchEntity);
        Task<MatchEntity> ToMatchEntityAsync(MatchViewModel model, bool isNew);

        Task<GroupEntity> GetFindGroupsAsync(int id);


        Task<CloseMatchViewModel> GetCloseMatchViewModel(CloseMatchViewModel closeMatch);
        Task CloseMatchAsync(int matchId, int goalsLocal, int goalsVisitor);
        Task<MatchEntity> GetMatchDataAsync(MatchEntity match);
    }
}
