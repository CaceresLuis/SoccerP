using Soccer.Web.Models;
using Soccer.Web.Helpers;
using System.Threading.Tasks;
using Soccer.Web.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Soccer.Web.Services.TournamentService;
using Microsoft.AspNetCore.Authorization;

namespace Soccer.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class TournamentsController : Controller
    {
        private readonly IImageHelper _imageHelper;
        private readonly ITournamentService _tournamentService;

        public TournamentsController(
            IImageHelper imageHelper,
            ITournamentService tournamentService)
        {
            _imageHelper = imageHelper;
            _tournamentService = tournamentService;
        }

        public async Task<IActionResult> Index()
        {
            if (TempData["mensaje"] != null)
                ViewBag.mensaje = TempData["mensaje"].ToString();
            return View(await _tournamentService.GetTeamListAsync());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TournamentViewModel model)
        {
            if (ModelState.IsValid)
            {
                string path = string.Empty;

                if (model.LogoFile != null)
                {
                    path = await _imageHelper.UploadImageAsync(model.LogoFile, "Tournaments");
                }

                TournamentEntity tournament = _tournamentService.ToTournamentEntity(model, path, true);
                await _tournamentService.AddTournamentAsync(tournament);
                TempData["mensaje"] = $"El torneo {tournament.Name} creado correctamente";
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            if (id < 1) return NotFound();

            TournamentEntity tournamentEntity = await _tournamentService.GetTournamentDetailsAsync(id);

            if (tournamentEntity == null) return NotFound();
            if (TempData["mensaje"] != null)
                ViewBag.mensaje = TempData["mensaje"].ToString();

            return View(tournamentEntity);
        }


        public async Task<IActionResult> Edit(int id)
        {
            if (id < 1) return NotFound();

            TournamentEntity tournamentEntity = await _tournamentService.GetTournamentFindAsync(id);
            if (tournamentEntity == null) return NotFound();

            TournamentViewModel model = _tournamentService.ToTournamentViewModel(tournamentEntity);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(TournamentViewModel model)
        {
            if (ModelState.IsValid)
            {
                string path = model.LogoPath;

                if (model.LogoFile != null)
                {
                    path = await _imageHelper.UploadImageAsync(model.LogoFile, "Tournaments");
                }

                TournamentEntity tournamentEntity = _tournamentService.ToTournamentEntity(model, path, false);
                await _tournamentService.EditTournamentAsync(model);
                TempData["mensaje"] = $"El torneo {tournamentEntity.Name} se editado correctamente";
                return RedirectToAction("Details", new { id = tournamentEntity.Id });
            }

            return View(model);
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (id < 1) return NotFound();

            TournamentEntity tournamentEntity = await _tournamentService.GetTournamentFindAsync(id);
            if (tournamentEntity == null) return NotFound();

            var comprobate = _tournamentService.ComprobateDelete(tournamentEntity.Id);
            if (comprobate != "OK")
            {
                TempData["mensaje"] = $"{comprobate}";
                return RedirectToAction(nameof(Index));
            }

            await _tournamentService.DeleteTournamentAsync(tournamentEntity);
            TempData["mensaje"] = $"El torneo {tournamentEntity.Name} eliminado correctamente";
            return RedirectToAction(nameof(Index));
        }

    }
}