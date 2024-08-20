using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using XuongMay.Core.Utils;

namespace XuongMay.Contract.Repositories.Entity
{
    public class ApplicationUserClaims : IdentityUserClaim<Guid>
    {
        public string? CreatedBy { get; set; }
        public string? LastUpdatedBy { get; set; }
        public string? DeletedBy { get; set; }
        public DateTimeOffset CreatedTime { get; set; }
        public DateTimeOffset LastUpdatedTime { get; set; }
        public DateTimeOffset? DeletedTime { get; set; }
        public ApplicationUserClaims()
        {
            CreatedTime = CoreHelper.SystemTimeNow;
            LastUpdatedTime = CreatedTime;
        }
        public void SetAuditFields(ClaimsPrincipal user)
        {
            string? username = user.FindFirst(ClaimTypes.Name)?.Value;

            if (CreatedBy == null)
            {
                CreatedBy = username;
                CreatedTime = CoreHelper.SystemTimeNow;
            }

            LastUpdatedBy = username;
            LastUpdatedTime = CoreHelper.SystemTimeNow;
        }

        public void SetDeletedFields(ClaimsPrincipal user)
        {
            DeletedBy = user.FindFirst(ClaimTypes.Name)?.Value;
            DeletedTime = CoreHelper.SystemTimeNow;
        }
    }
}
