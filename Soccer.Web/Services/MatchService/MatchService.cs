using Microsoft.EntityFrameworkCore;
using Soccer.Common.Enums;
using Soccer.Web.Data;
using Soccer.Web.Data.Entities;
using Soccer.Web.Helpers;
using Soccer.Web.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Soccer.Web.Services.MatchService
{
    public class MatchService : IMatchService
    {
        private readonly DataContext _context;
        private readonly ICombosHelper _combosHelper;
        private  MatchEntity _matchEntity;
        private  MatchStatus _matchStatus;

        public MatchService(
            DataContext context,
            ICombosHelper combosHelper)
        {
            _context = context;
           _combosHelper = combosHelper;
        }

        public async Task<MatchEntity> GetMatchGroupAsync(int id)
        {
            return await _context.Matches
                .Include(m => m.Group)
                .Include(m => m.Local)
                .Include(m => m.Visitor)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<MatchEntity> GetMatchDataAsync(MatchEntity match)
        {
            var data = await _context.Matches
                .Include(m => m.Local)
                .Include(m => m.Visitor)
                .Where(m => m.DateLocal == match.Date)
                .FirstOrDefaultAsync();

            return (data);
        }

        public async Task<MatchEntity> GetMatchAsync(int id)
        {
            return await _context.Matches
                .Include(m => m.Group)
                .Include(m => m.Local)
                .Include(m => m.Visitor)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<MatchEntity> AddMatchAsync(MatchEntity match)
        {
            _context.Add(match);
            await _context.SaveChangesAsync();
            return (match);
        }

        public async Task EditMatchAsync(int matchId, int goalsLocal, int goalsVisitor)
        {
            _matchEntity = await _context.Matches
                .Include(m => m.Local)
                .Include(m => m.Visitor)
                .Include(m => m.Predictions)
                .Include(m => m.Group)
                .ThenInclude(g => g.GroupDetails)
                .ThenInclude(gd => gd.Team)
                .FirstOrDefaultAsync(m => m.Id == matchId);

            _matchStatus = GetMatchStaus(_matchEntity.GoalsLocal.Value, _matchEntity.GoalsVisitor.Value);

            UpdatePositions(false);
            await _context.SaveChangesAsync();

            _matchEntity.GoalsLocal = goalsLocal;
            _matchEntity.GoalsVisitor = goalsVisitor;
            _matchEntity.IsClosed = true;
            _matchStatus = GetMatchStaus(_matchEntity.GoalsLocal.Value, _matchEntity.GoalsVisitor.Value);

            UpdatePositions(true);
            await _context.SaveChangesAsync();
        }

        public async Task<MatchEntity> DeleteGroupAsync(MatchEntity match)
        {
            _context.Remove(match);
            await _context.SaveChangesAsync();
            return match;
        }


        public async Task<MatchEntity> ToMatchEntityAsync(MatchViewModel model, bool isNew)
        {
            return new MatchEntity
            {
                Date = model.Date.ToUniversalTime(),
                GoalsLocal = model.GoalsLocal,
                GoalsVisitor = model.GoalsVisitor,
                Group = await _context.Groups.FindAsync(model.GroupId),
                Id = isNew ? 0 : model.Id,
                IsClosed = model.IsClosed,
                Local = await _context.Teams.FindAsync(model.LocalId),
                Visitor = await _context.Teams.FindAsync(model.VisitorId)
            };
        }

        public MatchViewModel ToMatchViewModel(MatchEntity matchEntity)
        {
            return new MatchViewModel
            {
                Date = matchEntity.Date.ToLocalTime(),
                GoalsLocal = matchEntity.GoalsLocal,
                GoalsVisitor = matchEntity.GoalsVisitor,
                Group = matchEntity.Group,
                GroupId = matchEntity.Group.Id,
                Id = matchEntity.Id,
                IsClosed = matchEntity.IsClosed,
                Local = matchEntity.Local,
                LocalId = matchEntity.Local.Id,
                Teams = _combosHelper.GetComboTeams(matchEntity.Group.Id),
                Visitor = matchEntity.Visitor,
                VisitorId = matchEntity.Visitor.Id
            };
        }
        public async Task<GroupEntity> GetFindGroupsAsync(int id)
        {
            return await _context.Groups.FindAsync(id);
        }

        public async Task CloseMatchAsync(int matchId, int goalsLocal, int goalsVisitor)
        {
            _matchEntity = await _context.Matches
                .Include(m => m.Local)
                .Include(m => m.Visitor)
                .Include(m => m.Predictions)
                .Include(m => m.Group)
                .ThenInclude(g => g.GroupDetails)
                .ThenInclude(gd => gd.Team)
                .FirstOrDefaultAsync(m => m.Id == matchId);

            _matchEntity.GoalsLocal = goalsLocal;
            _matchEntity.GoalsVisitor = goalsVisitor;
            _matchEntity.IsClosed = true;
            _matchStatus = GetMatchStaus(_matchEntity.GoalsLocal.Value, _matchEntity.GoalsVisitor.Value);

            UpdatePointsInpredictions();
            UpdatePositions(true);

            await _context.SaveChangesAsync();
        }

        private void UpdatePointsInpredictions()
        {
            foreach (PredictionEntity predictionEntity in _matchEntity.Predictions)
            {
                predictionEntity.Points = GetPoints(predictionEntity);
            }
        }

        private int GetPoints(PredictionEntity predictionEntity)
        {
            int points = 0;
            if (predictionEntity.GoalsLocal == _matchEntity.GoalsLocal)
            {
                points += 2;
            }

            if (predictionEntity.GoalsVisitor == _matchEntity.GoalsVisitor)
            {
                points += 2;
            }

            if (_matchStatus == GetMatchStaus(predictionEntity.GoalsLocal.Value, predictionEntity.GoalsVisitor.Value))
            {
                points++;
            }

            return points;
        }

        private MatchStatus GetMatchStaus(int goalsLocal, int goalsVisitor)
        {
            if (goalsLocal > goalsVisitor)
                return MatchStatus.LocalWin;

            if (goalsVisitor > goalsLocal)
                return MatchStatus.VisitorWin;

            return MatchStatus.Tie;
        }

        private void UpdatePositions(bool EdipOrUpdate)
        {
            if(EdipOrUpdate == true)
            {
                GroupDetailEntity local = _matchEntity.Group.GroupDetails.FirstOrDefault(gd => gd.Team == _matchEntity.Local);
                GroupDetailEntity visitor = _matchEntity.Group.GroupDetails.FirstOrDefault(gd => gd.Team == _matchEntity.Visitor);

                local.MatchesPlayed++;
                visitor.MatchesPlayed++;

                local.GoalsFor += _matchEntity.GoalsLocal.Value;
                local.GoalsAgainst += _matchEntity.GoalsVisitor.Value;
                visitor.GoalsFor += _matchEntity.GoalsVisitor.Value;
                visitor.GoalsAgainst += _matchEntity.GoalsLocal.Value;

                if (_matchStatus == MatchStatus.LocalWin)
                {
                    local.MatchesWon++;
                    visitor.MatchesLost++;
                }
                else if (_matchStatus == MatchStatus.VisitorWin)
                {
                    visitor.MatchesWon++;
                    local.MatchesLost++;
                }
                else
                {
                    local.MatchesTied++;
                    visitor.MatchesTied++;
                }
            }
            else
            {
                GroupDetailEntity local = _matchEntity.Group.GroupDetails.FirstOrDefault(gd => gd.Team == _matchEntity.Local);
                GroupDetailEntity visitor = _matchEntity.Group.GroupDetails.FirstOrDefault(gd => gd.Team == _matchEntity.Visitor);

                local.MatchesPlayed--;
                visitor.MatchesPlayed--;

                local.GoalsFor -= _matchEntity.GoalsLocal.Value;
                local.GoalsAgainst -= _matchEntity.GoalsVisitor.Value;
                visitor.GoalsFor -= _matchEntity.GoalsVisitor.Value;
                visitor.GoalsAgainst -= _matchEntity.GoalsLocal.Value;

                if (_matchStatus == MatchStatus.LocalWin)
                {
                    local.MatchesWon--;
                    visitor.MatchesLost--;
                }
                else if (_matchStatus == MatchStatus.VisitorWin)
                {
                    visitor.MatchesWon--;
                    local.MatchesLost--;
                }
                else
                {
                    local.MatchesTied--;
                    visitor.MatchesTied--;
                }
            }
        }

        public async Task<CloseMatchViewModel> GetCloseMatchViewModel(CloseMatchViewModel closeMatch)
        {
            closeMatch.Group = await _context.Groups.FindAsync(closeMatch.GroupId);
            closeMatch.Local = await _context.Teams.FindAsync(closeMatch.LocalId);
            closeMatch.Visitor = await _context.Teams.FindAsync(closeMatch.VisitorId);
            return closeMatch;
        }
    }
}
