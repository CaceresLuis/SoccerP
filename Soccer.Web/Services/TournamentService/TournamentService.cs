using Microsoft.EntityFrameworkCore;
using Soccer.Common.Models;
using Soccer.Web.Data;
using Soccer.Web.Data.Entities;
using Soccer.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Soccer.Web.Services.TournamentService
{
    public class TournamentService : ITournamentService
    {
        private readonly DataContext _context;

        public TournamentService(DataContext context)
        {
            _context = context;
        }

        public async Task<TournamentEntity> AddTournamentAsync(TournamentEntity tournament)
        {
            _context.Add(tournament);
            await _context.SaveChangesAsync();
            return (tournament);
        }

        public async Task<TournamentEntity> EditTournamentAsync(TournamentEntity tournament)
        {
            _context.Update(tournament);
            await _context.SaveChangesAsync();
            return (tournament);
        }

        public async Task<TournamentEntity> GetTournamentFindAsync(int id)
        {
            return await _context.Tournaments.FindAsync(id);
        }

        public async Task<TournamentEntity> GetTournamentDetailsAsync(int id)
        {
            return await _context.Tournaments
                .Include(t => t.Groups)
                .ThenInclude(t => t.Matches)
                .ThenInclude(t => t.Local)
                .Include(t => t.Groups)
                .ThenInclude(t => t.Matches)
                .ThenInclude(t => t.Visitor)
                .Include(t => t.Groups)
                .ThenInclude(t => t.GroupDetails)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<TournamentEntity[]> GetTeamListAsync()
        {
            return await _context.Tournaments
                .Include(t => t.Groups)
                .OrderBy(t => t.StartDate)
                .ToArrayAsync();
        }

        public string ComprobateDelete(int idTournament)
        {
            try
            {
                var group = "";
                var groupDetil = _context.Tournaments
                    .Include(t => t.Groups)
                    .FirstOrDefault(g => g.Id == idTournament);
                if (groupDetil.Groups != null)
                {
                    group = $"El torneo {groupDetil.Name} tiene grupos";
                    return (group);
                }
            }
            catch (System.Exception)
            {
                return "OK";
            }
            return "OK";
        }

        public async Task<TournamentEntity> DeleteTournamentAsync(TournamentEntity tournament)
        {
            _context.Tournaments.Remove(tournament);
            await _context.SaveChangesAsync();
            return (tournament);
        }


        public TournamentEntity ToTournamentEntity(TournamentViewModel model, string path, bool isNew)
        {
            return new TournamentEntity
            {
                EndDate = model.EndDate.ToUniversalTime(),
                Groups = model.Groups,
                Id = isNew ? 0 : model.Id,
                IsActive = model.IsActive,
                LogoPath = path,
                Name = model.Name,
                StartDate = model.StartDate.ToUniversalTime()
            };
        }

        public TournamentViewModel ToTournamentViewModel(TournamentEntity tournamentEntity)
        {
            return new TournamentViewModel
            {
                EndDate = tournamentEntity.EndDate,
                Groups = tournamentEntity.Groups,
                Id = tournamentEntity.Id,
                IsActive = tournamentEntity.IsActive,
                LogoPath = tournamentEntity.LogoPath,
                Name = tournamentEntity.Name,
                StartDate = tournamentEntity.StartDate
            };
        }

        //--------
        public TournamentResponse ToTournamentResponse(TournamentEntity tournamentEntity)
        {
            return new TournamentResponse
            {
                EndDate = tournamentEntity.EndDate,
                Id = tournamentEntity.Id,
                IsActive = tournamentEntity.IsActive,
                LogoPath = tournamentEntity.LogoPath,
                Name = tournamentEntity.Name,
                StartDate = tournamentEntity.StartDate,
                Groups = tournamentEntity.Groups?.Select(g => new GroupResponse
                {
                    Id = g.Id,
                    Name = g.Name,
                    GroupDetails = g.GroupDetails?.Select(gd => new GroupDetailResponse
                    {
                        GoalsAgainst = gd.GoalsAgainst,
                        GoalsFor = gd.GoalsFor,
                        Id = gd.Id,
                        MatchesLost = gd.MatchesLost,
                        MatchesPlayed = gd.MatchesPlayed,
                        MatchesTied = gd.MatchesTied,
                        MatchesWon = gd.MatchesWon,
                        Team = ToTeamResponse(gd.Team)
                    }).ToList(),
                    Matches = g.Matches?.Select(m => new MatchResponse
                    {
                        Date = m.Date,
                        GoalsLocal = m.GoalsLocal,
                        GoalsVisitor = m.GoalsVisitor,
                        Id = m.Id,
                        IsClosed = m.IsClosed,
                        Local = ToTeamResponse(m.Local),
                        Visitor = ToTeamResponse(m.Visitor),
                        Predictions = m.Predictions?.Select(p => new PredictionResponse
                        {
                            GoalsLocal = p.GoalsLocal,
                            GoalsVisitor = p.GoalsVisitor,
                            Id = p.Id,
                            Points = p.Points,
                            User = ToUserResponse(p.User)
                        }).ToList()
                    }).ToList()
                }).ToList()
            };
        }

        public List<TournamentResponse> ToTournamentResponse(List<TournamentEntity> tournamentEntities)
        {
            List<TournamentResponse> list = new List<TournamentResponse>();
            foreach (TournamentEntity tournamentEntity in tournamentEntities)
            {
                list.Add(ToTournamentResponse(tournamentEntity));
            }

            return list;
        }

        public UserResponse ToUserResponse(UserEntity user)
        {
            if (user == null) return null;

            return new UserResponse
            {
                Address = user.Address,
                Document = user.Document,
                Email = user.Email,
                FirstName = user.FirstName,
                Id = user.Id,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                PicturePath = user.PicturePath,
                Team = ToTeamResponse(user?.Team),
                UserType = user.UserType
            };
        }

        private TeamResponse ToTeamResponse(TeamEntity team)
        {
            if (team == null) return null;

            return new TeamResponse
            {
                Id = team.Id,
                LogoPath = team.LogoPath,
                Name = team.Name
            };
        }

        public async Task<List<TournamentEntity>> GetTournamentsListAsync()
        {
            return (await _context.Tournaments
            .Include(t => t.Groups)
            .ThenInclude(g => g.GroupDetails)
            .ThenInclude(gd => gd.Team)
            .Include(t => t.Groups)
            .ThenInclude(g => g.Matches)
            .ThenInclude(m => m.Local)
            .Include(t => t.Groups)
            .ThenInclude(g => g.Matches)
            .ThenInclude(m => m.Visitor)
            .ToListAsync());
        }

        public async Task<TournamentEntity> GetTournamentForAPI(int id)
        {
           return await _context.Tournaments
                .Include(t => t.Groups)
                .ThenInclude(g => g.Matches)
                .ThenInclude(m => m.Predictions)
                .ThenInclude(p => p.User)
                .ThenInclude(u => u.Team)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<List<UserEntity>> GetListUsersAsync()
        {
            return await _context.Users
                .Include(u => u.Team)
                .Include(u => u.Predictions)
                .ToListAsync();
        }
    }
}