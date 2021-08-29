using Soccer.Web.Models;
using Soccer.Web.Helpers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Soccer.Web.Services.GroupService;
using Microsoft.AspNetCore.Authorization;

namespace Soccer.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class GroupsController : Controller
    {
        private readonly IGroupService _groupService;
        private readonly ICombosHelper _combosHelper;

        public GroupsController(
            IGroupService groupService,
            ICombosHelper combosHelper)
        {
            _groupService = groupService;
            _combosHelper = combosHelper;
        }

        public async Task<IActionResult> AddGroup(int id)
        {
            if (id < 1) return NotFound();

            Data.Entities.TournamentEntity tournamentEntity = await _groupService.GetTournamentFindAsync(id);
            if (tournamentEntity == null) return NotFound();

            GroupViewModel model = new GroupViewModel
            {
                Tournament = tournamentEntity,
                TournamentId = tournamentEntity.Id
            };

            TempData["mensaje"] = $"Grupo Creado correctamente";
            return View(model);
        }

        public async Task<IActionResult> EditGroup(int id)
        {
            if (id < 1) return NotFound();

            var groupEntity = await _groupService.GetGroupTournamentsAsync(id);

            if (groupEntity == null) return NotFound();

            var model = _groupService.ToGroupViewModel(groupEntity);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrUpdateGroup(GroupViewModel model, bool isNew)
        {
            if (ModelState.IsValid)
            {
                if (isNew == true)
                {
                    var group = await _groupService.ToGroupEntityAsync(model, isNew);
                    TempData["mensaje"] = $"El Grupo {group.Name} se ha creado";
                    await _groupService.AddGroupAsync(group);
                    return RedirectToAction("Details", "Tournaments", new { id = model.TournamentId });
                }

                var groupEntity = await _groupService.ToGroupEntityAsync(model, isNew);

                await _groupService.EditGroupAsync(groupEntity);
                TempData["mensaje"] = $"El grupo {groupEntity.Name} se ha modificado";
                return RedirectToAction(nameof(DetailsGroup), new { id = model.Id });
            }
            return View(model);
        }

        public async Task<IActionResult> DetailsGroup(int id)
        {
            if (id < 1) return NotFound();

            var groupEntity = await _groupService.GetFullGroupAsync(id);

            if (groupEntity == null) return NotFound();

            if (TempData["mensaje"] != null)
                ViewBag.mensaje = TempData["mensaje"].ToString();
            return View(groupEntity);
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (id < 1) return NotFound();
            var groupEntity = await _groupService.GetGroupTournamentsAsync(id);
            if (groupEntity == null) return NotFound();

            var comprobate = _groupService.ComprobateDelete(groupEntity.Id);
            if (comprobate != "OK")
            {
                TempData["mensaje"] = $"{comprobate}";
                return RedirectToAction("Details", "Tournaments", new { id = groupEntity.Tournament.Id });
            }

            await _groupService.DeleteGroupAsync(groupEntity);
            TempData["mensaje"] = "Grupo eliminado correctamente";
            return RedirectToAction("Details", "Tournaments", new { id = groupEntity.Tournament.Id });
        }
    }
}