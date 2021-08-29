using Soccer.Common.Models;
using Soccer.Web.Data.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Soccer.Web.Services.PredictionService
{
    public interface IPredictionService
    {
        Task<UserEntity> GetUserAsync(Guid userId);
        Task<MatchEntity> GetFindMatchAsync(int id);
        Task<List<MatchEntity>> GetMatchAsync(int id);
        Task AddPredictionAsync(PredictionEntity prediction);
        Task<TournamentEntity> GetTournamentFindAsync(int id);
        MatchResponse ToMatchResponse(MatchEntity matchEntity);
        Task<UserEntity> GetFullUserForApiAsync(string UserId);
        Task UpdatePredictionAsync(PredictionEntity prediction);
        Task<PredictionEntity> GetFirsPredictionAsync(Guid userId, int matchId);
        PredictionResponse ToPredictionResponse(PredictionEntity predictionEntity);
    }
}