using Soccer.Common.Models;
using Soccer.Web.Data.Entities;
using Soccer.Web.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Soccer.Web.Services.TournamentService
{
    public interface ITournamentService
    {
        Task<List<UserEntity>> GetListUsersAsync();
        Task<TournamentEntity[]> GetTeamListAsync();
        Task<TournamentEntity> GetTournamentFindAsync(int id);
        Task<List<TournamentEntity>> GetTournamentsListAsync();
        Task<TournamentEntity> GetTournamentDetailsAsync(int id);
        Task<TournamentEntity> AddTournamentAsync(TournamentEntity tournament);
        Task<TournamentEntity> EditTournamentAsync(TournamentEntity tournament);
        Task<TournamentEntity> DeleteTournamentAsync(TournamentEntity tournament);

        TournamentEntity ToTournamentEntity(TournamentViewModel model, string path, bool isNew);
        TournamentViewModel ToTournamentViewModel(TournamentEntity tournamentEntity);
       
        TournamentResponse ToTournamentResponse(TournamentEntity tournamentEntity);
        List<TournamentResponse> ToTournamentResponse(List<TournamentEntity> tournamentEntities);
        Task<TournamentEntity> GetTournamentForAPI(int id);
        UserResponse ToUserResponse(UserEntity user);
        string ComprobateDelete(int idTournament);
    }
}