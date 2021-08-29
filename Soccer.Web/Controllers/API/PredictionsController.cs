using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Soccer.Common.Models;
using Soccer.Web.Data.Entities;
using Soccer.Web.Resources;
using Soccer.Web.Services.PredictionService;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Soccer.Web.Controllers.API
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class PredictionsController : ControllerBase
    {
        private readonly IPredictionService _predictionService;

        public PredictionsController(IPredictionService predictionService)
        {
            _predictionService = predictionService;
        }

        [HttpPost]
        public async Task<IActionResult> PostPrediction([FromBody] PredictionRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            CultureInfo cultureInfo = new CultureInfo(request.CultureInfo);
            Resource.Culture = cultureInfo;

            MatchEntity matchEntity = await _predictionService.GetFindMatchAsync(request.MatchId);
            if (matchEntity == null) return BadRequest(Resource.MatchDoesntExists);

            if (matchEntity.IsClosed) return BadRequest(Resource.MatchAlreadyClosed);

            UserEntity userEntity = await _predictionService.GetUserAsync(request.UserId);
            if (userEntity == null) return BadRequest(Resource.UserDoesntExists);

            if (matchEntity.Date <= DateTime.UtcNow) return BadRequest(Resource.MatchAlreadyStarts);

            PredictionEntity predictionEntity = await _predictionService.GetFirsPredictionAsync
                (request.UserId, request.MatchId);

            if (predictionEntity == null)
            {
                predictionEntity = new PredictionEntity
                {
                    GoalsLocal = request.GoalsLocal,
                    GoalsVisitor = request.GoalsVisitor,
                    Match = matchEntity,
                    User = userEntity
                };

                await _predictionService.AddPredictionAsync(predictionEntity);
            }
            else
            {
                predictionEntity.GoalsLocal = request.GoalsLocal;
                predictionEntity.GoalsVisitor = request.GoalsVisitor;
                await _predictionService.UpdatePredictionAsync(predictionEntity);
            }
            return NoContent();
        }

        [HttpPost]
        [Route("GetPredictionsForUser")]
        public async Task<IActionResult> GetPredictionsForUser([FromBody] PredictionsForUserRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            CultureInfo cultureInfo = new CultureInfo(request.CultureInfo);
            Resource.Culture = cultureInfo;

            TournamentEntity tournament = await _predictionService.GetTournamentFindAsync(request.TournamentId);
            if (tournament == null) return BadRequest(Resource.TournamentDoesntExists);

            UserEntity userEntity = await _predictionService.GetFullUserForApiAsync(request.UserId.ToString());
            if (userEntity == null) return BadRequest(Resource.UserDoesntExists);

            // Add precitions already done
            List<PredictionResponse> predictionResponses = new List<PredictionResponse>();
            foreach (PredictionEntity predictionEntity in userEntity.Predictions)
            {
                if (predictionEntity.Match.Group.Tournament.Id == request.TournamentId)
                {
                    predictionResponses.Add(_predictionService.ToPredictionResponse(predictionEntity));
                }
            }

            // Add precitions undone
            List<MatchEntity> matches = await _predictionService.GetMatchAsync(request.TournamentId);
            foreach (MatchEntity matchEntity in matches)
            {
                PredictionResponse predictionResponse = predictionResponses.FirstOrDefault(pr => pr.Match.Id == matchEntity.Id);
                if (predictionResponse == null)
                {
                    predictionResponses.Add(new PredictionResponse
                    {
                        Match = _predictionService.ToMatchResponse(matchEntity),
                    });
                }
            }

            return Ok(predictionResponses.OrderBy(pr => pr.Id).ThenBy(pr => pr.Match.Date));
        }
    }
}