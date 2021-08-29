using Soccer.Web.Models;
using Soccer.Web.Helpers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Soccer.Web.Services.GroupDetail;
using Microsoft.AspNetCore.Authorization;

namespace Soccer.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class GroupDetailsController : Controller
    {
        private readonly ICombosHelper _combosHelper;
        private readonly IGroupDetailService _groupDetail;

        public GroupDetailsController(
            ICombosHelper combosHelper,
            IGroupDetailService groupDetail)
        {
            _groupDetail = groupDetail;
            _combosHelper = combosHelper;
        }
        public async Task<IActionResult> EditGroupDetail(int id)
        {
            if (id < 1) return NotFound();

            Data.Entities.GroupDetailEntity groupDetailEntity = await _groupDetail.GetGroupDetailsAsync(id);
            if (groupDetailEntity == null) return NotFound();

            GroupDetailViewModel model = await _groupDetail.ToGroupDetailViewModel(groupDetailEntity);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrUpdateGroupDetail(GroupDetailViewModel model, bool isNew)
        {
            if (ModelState.IsValid)
            {
                if(isNew == true)
                {
                    var groupDetail = await _groupDetail.ToGroupDetailEntityAsync(model, isNew);

                    TempData["mensaje"] = $"El team {groupDetail.Team.Name} se ha creado";
                    await _groupDetail.AddOrUpdateGroupDetailsAsync(groupDetail, isNew);
                    return RedirectToAction("DetailsGroup", "Groups", new { id = model.GroupId });
                }

                var groupDetailEntity = await _groupDetail.ToGroupDetailEntityAsync(model, isNew);

                TempData["mensaje"] = $"El team {groupDetailEntity.Team.Name} se ha Editado";
                await _groupDetail.AddOrUpdateGroupDetailsAsync(groupDetailEntity, isNew);
                return RedirectToAction("DetailsGroup", "Groups", new { id = model.GroupId });
            }

            model.SelectTeam = _combosHelper.GetComboTeams();
            return View(model);
        }

        public async Task<IActionResult> AddGroupDetail(int id)
        {
            if (id < 1) return NotFound();

            Data.Entities.GroupEntity groupEntity = await _groupDetail.GetFindGroupsAsync(id);
            if (groupEntity == null) return NotFound();

            GroupDetailViewModel model = new GroupDetailViewModel
            {
                Group = groupEntity,
                GroupId = groupEntity.Id,
                SelectTeam = _combosHelper.GetComboTeams()
            };

            return View(model);
        }

        public async Task<IActionResult> DeleteGroupDetail(int id)
        {
            if (id < 1) return NotFound();

            var groupDetailEntity = await _groupDetail.GetGroupDetailsAsync(id);
            if (groupDetailEntity == null) return NotFound();

            await _groupDetail.DeleteGroupDetailsAsync(groupDetailEntity);
            TempData["mensaje"] = $"El Team {groupDetailEntity.Team.Name} se ha eliminado";
            return RedirectToAction("DetailsGroup", "Groups", new { id = groupDetailEntity.Group.Id });
        }
    }
}