using Microsoft.AspNetCore.Mvc;
using Soccer.Common.Models;
using Soccer.Web.Data.Entities;
using Soccer.Web.Services.TournamentService;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Soccer.Web.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class TournamentsController : ControllerBase
    {
        private readonly ITournamentService _tournamentService;

        public TournamentsController(ITournamentService tournamentService)
        {
            _tournamentService = tournamentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetTeams()
        {
            List<TournamentEntity> tournaments =
                await _tournamentService.GetTournamentsListAsync();
            return Ok(_tournamentService.ToTournamentResponse(tournaments));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPositionsByTournament([FromRoute] int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            TournamentEntity tournament = await _tournamentService.GetTournamentForAPI(id);
            if (tournament == null) return BadRequest("Tournament doesn't exists.");

            List<PositionResponse> positionResponses = new List<PositionResponse>();
            foreach (GroupEntity groupEntity in tournament.Groups)
            {
                foreach (MatchEntity matchEntity in groupEntity.Matches)
                {
                    foreach (PredictionEntity predictionEntity in matchEntity.Predictions)
                    {
                        PositionResponse positionResponse = positionResponses.FirstOrDefault(pr => pr.UserResponse.Id == predictionEntity.User.Id);
                        if (positionResponse == null)
                        {
                            positionResponses.Add(new PositionResponse
                            {
                                Points = predictionEntity.Points,
                                UserResponse = _tournamentService.ToUserResponse(predictionEntity.User),
                            });
                        }
                        else
                        {
                            positionResponse.Points += predictionEntity.Points;
                        }
                    }
                }
            }

            List<PositionResponse> list = positionResponses.OrderByDescending(pr => pr.Points).ToList();
            int i = 1;
            foreach (PositionResponse item in list)
            {
                item.Ranking = i;
                i++;
            }
            return Ok(list);
        }

        [HttpGet]
        [Route("Positions")]
        public async Task<IActionResult> GetPositions()
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            List<UserEntity> users = await _tournamentService.GetListUsersAsync();
            List<PositionResponse> positionResponses = users.Select(u => new PositionResponse
            {
                Points = u.Points,
                UserResponse = _tournamentService.ToUserResponse(u)

            }).ToList();

            List<PositionResponse> list = positionResponses.OrderByDescending(pr => pr.Points).ToList();
            int i = 1;
            foreach (PositionResponse item in list)
            {
                item.Ranking = i;
                i++;
            }

            return Ok(list);
        }

        [HttpGet]
        [Route("GetTournaments")]
        public async Task<IActionResult> GetTournaments()
        {
            List<TournamentEntity> tournaments = await _tournamentService.GetTournamentsListAsync();
            return Ok(_tournamentService.ToTournamentResponse(tournaments));
        }
    }
}