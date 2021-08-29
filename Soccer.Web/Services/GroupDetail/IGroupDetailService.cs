using Soccer.Web.Data.Entities;
using Soccer.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Soccer.Web.Services.GroupDetail
{
    public interface IGroupDetailService
    {
        Task<GroupEntity> GetFindGroupsAsync(int id);

        Task<GroupDetailEntity> GetGroupDetailsAsync(int id);
        //Task<GroupDetailEntity> AddGroupDetailAsync(GroupDetailEntity groupDetail);
        //Task<GroupDetailEntity> EditGroupDetailsAsync(GroupDetailEntity groupDetail);
        Task<GroupDetailEntity> DeleteGroupDetailsAsync(GroupDetailEntity groupDetail);
        
        Task<GroupDetailEntity> ToGroupDetailEntityAsync(GroupDetailViewModel model, bool isNew);
       Task<GroupDetailViewModel> ToGroupDetailViewModel(GroupDetailEntity groupDetailEntity);
        Task<GroupDetailEntity> AddOrUpdateGroupDetailsAsync(GroupDetailEntity groupDetail, bool isNew);
    }
}
