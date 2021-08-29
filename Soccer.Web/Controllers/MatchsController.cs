using Soccer.Web.Models;
using Soccer.Web.Helpers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Soccer.Web.Data.Entities;
using Soccer.Web.Services.MatchService;
using Microsoft.AspNetCore.Authorization;

namespace Soccer.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class MatchsController : Controller
    {
        private readonly IMatchService _matchService;
        private readonly ICombosHelper _combosHelper;

        public MatchsController(
            IMatchService matchService,
            ICombosHelper combosHelper)
        {
            _matchService = matchService;
            _combosHelper = combosHelper;
        }
        public async Task<IActionResult> AddMatch(int id)
        {
            if (id < 1) return NotFound();

            GroupEntity groupEntity = await _matchService.GetFindGroupsAsync(id);
            if (groupEntity == null) return NotFound();

            MatchViewModel model = new MatchViewModel
            {
                Group = groupEntity,
                GroupId = groupEntity.Id,
                Teams = _combosHelper.GetComboTeams(groupEntity.Id)
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddMatch(MatchViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.LocalId != model.VisitorId)
                {
                    try
                    {
                        var data = await _matchService.GetMatchDataAsync(model);

                        if (data.Local.Id == model.LocalId || data.Visitor.Id == model.VisitorId)
                        {
                            ModelState.AddModelError(string.Empty, "Uno o ambos equipos ya juega este dia");
                            model.Group = await _matchService.GetFindGroupsAsync(model.GroupId);
                            model.Teams = _combosHelper.GetComboTeams(model.GroupId);
                            return View(model);
                        }
                    }
                    catch (System.Exception)
                    {
                        MatchEntity matchEntity = await _matchService.ToMatchEntityAsync(model, true);
                        await _matchService.AddMatchAsync(matchEntity);
                        TempData["mensaje"] = $"El match {matchEntity.Local.Name} vs {matchEntity.Visitor.Name} se ha creado";
                        return RedirectToAction("DetailsGroup", "Groups", new { id = model.GroupId });
                    }
                }
                ModelState.AddModelError(string.Empty, "The local and visitor must be differents teams.");
            }

            model.Group = await _matchService.GetFindGroupsAsync(model.GroupId);
            model.Teams = _combosHelper.GetComboTeams(model.GroupId);
            return View(model);
        }

        public async Task<IActionResult> EditMatch(int id)
        {
            if (id < 1) return NotFound();

            var matchEntity = await _matchService.GetMatchAsync(id);
            if (matchEntity == null) return NotFound();

            var model = new CloseMatchViewModel
            {
                Group = matchEntity.Group,
                GroupId = matchEntity.Group.Id,
                Local = matchEntity.Local,
                LocalId = matchEntity.Local.Id,
                MatchId = matchEntity.Id,
                Visitor = matchEntity.Visitor,
                VisitorId = matchEntity.Visitor.Id,
                GoalsLocal = matchEntity.GoalsLocal,
                GoalsVisitor = matchEntity.GoalsVisitor

            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditMatch(CloseMatchViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _matchService.EditMatchAsync(model.MatchId, model.GoalsLocal.Value, model.GoalsVisitor.Value);
                TempData["mensaje"] = $"Match Actualizado a {model.GoalsLocal} para el local y {model.GoalsVisitor} para el visitante";
                return RedirectToAction("DetailsGroup", "Groups", new { id = model.GroupId });
            }

            var modelOk = await _matchService.GetCloseMatchViewModel(model);
            return View(modelOk);
        }

        public async Task<IActionResult> DeleteMatch(int id)
        {
            if (id < 1) return NotFound();

            MatchEntity matchEntity = await _matchService.GetMatchGroupAsync(id);
            if (matchEntity == null) return NotFound();

            await _matchService.DeleteGroupAsync(matchEntity);
            TempData["mensaje"] = $"Match entre {matchEntity.Local.Name} y {matchEntity.Visitor.Name} se ha Cancelado";
            return RedirectToAction("DetailsGroup", "Groups", new { id = matchEntity.Group.Id });
        }


        public async Task<IActionResult> CloseMatch(int id)
        {
            if (id < 1) return NotFound();

            var matchEntity = await _matchService.GetMatchAsync(id);
            if (matchEntity == null)return NotFound();

            var model = new CloseMatchViewModel
            {
                Group = matchEntity.Group,
                GroupId = matchEntity.Group.Id,
                Local = matchEntity.Local,
                LocalId = matchEntity.Local.Id,
                MatchId = matchEntity.Id,
                Visitor = matchEntity.Visitor,
                VisitorId = matchEntity.Visitor.Id,
                GoalsLocal = matchEntity.GoalsLocal,
                GoalsVisitor = matchEntity.GoalsVisitor

            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CloseMatch(CloseMatchViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _matchService.CloseMatchAsync(model.MatchId, model.GoalsLocal.Value, model.GoalsVisitor.Value);
                TempData["mensaje"] = $"Match Cerrado con {model.GoalsLocal} para el local y {model.GoalsVisitor} para el visitante";
                return RedirectToAction("DetailsGroup", "Groups", new { id = model.GroupId });
            }

            var modelOk = await _matchService.GetCloseMatchViewModel(model);
            return View(modelOk);
        }
    }
}