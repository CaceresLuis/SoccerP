using System;
using Soccer.Web.Models;
using Soccer.Web.Helpers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Soccer.Web.Data.Entities;
using Soccer.Web.Services.TeamService;
using Microsoft.AspNetCore.Authorization;

namespace Soccer.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class TeamsController : Controller
    {
        private readonly ITeamService _teamService;
        private readonly IImageHelper _imageHelper;

        public TeamsController(
            ITeamService teamService,
            IImageHelper imageHelper)
        {
            _teamService = teamService;
            _imageHelper = imageHelper;
        }

        public async Task<IActionResult> Index()
        {
            if (TempData["mensaje"] != null)
                ViewBag.mensaje = TempData["mensaje"].ToString();
            return View(await _teamService.GetTeamList());
        }

        public IActionResult Create()
        {
            return View();
        }

        public async Task<IActionResult> Edit(int id)
        {
            if (id < 1) return NotFound();

            TeamEntity teamEntity = await _teamService.GetFindEntity(id);
            if (teamEntity == null) return NotFound();

            TeamViewModel teamViewModel = _teamService.ToTeamViewModel(teamEntity);

            return View(teamViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrUpdate(TeamViewModel teamViewModel, int id, bool isNew)
        {
            string path = string.Empty;
            string msj = "creado";
            if(isNew == false)
            {
                if (id != teamViewModel.Id) return NotFound();
                path = teamViewModel.LogoPath;
                msj = "modificado";
            }

            if (ModelState.IsValid)
            {
                if (teamViewModel.LogoFile != null)
                {
                    path = await _imageHelper.UploadImageAsync(teamViewModel.LogoFile, "Teams");
                }

                TeamEntity teamEntity = _teamService.ToTeamEntity(teamViewModel, path, isNew);

                try
                {
                    await _teamService.AddOrUpdateTeamAsync(teamEntity, isNew);
                    TempData["mensaje"] = $"El equipo {teamEntity.Name} se ha {msj}";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    if (ex.InnerException.Message.Contains("duplicate"))
                    {
                        ModelState.AddModelError(string.Empty, $"Already exists the team {teamEntity.Name}");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, ex.InnerException.Message);
                    }
                }
            }

            var redirec = isNew ? "Create" : $"Edit/{id}";
            return RedirectToAction($"{redirec}");
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (id < 1) return NotFound();

            TeamEntity teamEntity = await _teamService.GetFindEntity(id);
            if (teamEntity == null) return NotFound();

            var comprobate = _teamService.ComprobateDelete(id);
            if (comprobate != "OK")
            {
                TempData["mensaje"] = $"{comprobate}";
                return RedirectToAction(nameof(Index));
            }
               
            await _teamService.DeleteTeamAsync(teamEntity);
            TempData["mensaje"] = $"El equipo {teamEntity.Name} se ha eliminado";
            return RedirectToAction(nameof(Index));            
        }
    }
}