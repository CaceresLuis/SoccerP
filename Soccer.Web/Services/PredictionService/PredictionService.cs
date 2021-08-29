using Microsoft.EntityFrameworkCore;
using Soccer.Common.Models;
using Soccer.Web.Data;
using Soccer.Web.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Soccer.Web.Services.PredictionService
{
    public class PredictionService : IPredictionService
    {
        private readonly DataContext _context;

        public PredictionService(DataContext context)
        {
            _context = context;
        }

        public async Task<TournamentEntity> GetTournamentFindAsync(int id)
        {
            return await _context.Tournaments.FindAsync(id);
        }

        public PredictionResponse ToPredictionResponse(PredictionEntity predictionEntity)
        {
            return new PredictionResponse
            {
                GoalsLocal = predictionEntity.GoalsLocal,
                GoalsVisitor = predictionEntity.GoalsVisitor,
                Id = predictionEntity.Id,
                Match = ToMatchResponse(predictionEntity.Match),
                Points = predictionEntity.Points
            };
        }

        public MatchResponse ToMatchResponse(MatchEntity matchEntity)
        {
            return new MatchResponse
            {
                Date = matchEntity.Date,
                GoalsLocal = matchEntity.GoalsLocal,
                GoalsVisitor = matchEntity.GoalsVisitor,
                Id = matchEntity.Id,
                IsClosed = matchEntity.IsClosed,
                Local = ToTeamResponse(matchEntity.Local),
                Visitor = ToTeamResponse(matchEntity.Visitor)
            };
        }

        private TeamResponse ToTeamResponse(TeamEntity team)
        {
            if (team == null)
            {
                return null;
            }

            return new TeamResponse
            {
                Id = team.Id,
                LogoPath = team.LogoPath,
                Name = team.Name
            };
        }

        public async Task<UserEntity> GetFullUserForApiAsync(string UserId)
        {
            return( await _context.Users
                .Include(u => u.Team)
                .Include(u => u.Predictions)
                .ThenInclude(p => p.Match)
                .ThenInclude(m => m.Local)
                .Include(u => u.Predictions)
                .ThenInclude(p => p.Match)
                .ThenInclude(m => m.Visitor)
                .Include(u => u.Predictions)
                .ThenInclude(p => p.Match)
                .ThenInclude(p => p.Group)
                .ThenInclude(p => p.Tournament)
                .FirstOrDefaultAsync(u => u.Id == UserId.ToString()));
        }

        public async Task<List<MatchEntity>> GetMatchAsync(int TournamentId)
        {
            return( await _context.Matches
                .Include(m => m.Local)
                .Include(m => m.Visitor)
                .Where(m => m.Group.Tournament.Id == TournamentId)
                .ToListAsync());
        }

        public async Task<MatchEntity> GetFindMatchAsync(int id)
        {
            return await _context.Matches.FindAsync(id);
        }

        public async Task<UserEntity> GetUserAsync(Guid userId)
        {
            return await _context.Users
                .Include(u => u.Team)
                .FirstOrDefaultAsync(u => u.Id == userId.ToString());
        }

        public async Task<PredictionEntity> GetFirsPredictionAsync(Guid userId, int matchId)
        {
            return await _context.Predictions
                .FirstOrDefaultAsync(p => p.User.Id == userId.ToString()
                && p.Match.Id == matchId);
        }

        public async Task AddPredictionAsync(PredictionEntity prediction)
        {
            _context.Predictions.Add(prediction);
            await _context.SaveChangesAsync();
        }

        public async Task UpdatePredictionAsync(PredictionEntity prediction)
        {
            _context.Predictions.Update(prediction);
            await _context.SaveChangesAsync();
        }
    }
}