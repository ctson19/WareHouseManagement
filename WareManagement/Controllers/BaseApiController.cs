using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WareManagement.Controllers;

[Authorize]
public abstract class BaseApiController : ControllerBase
{
    protected int GetCurrentUserId()
    {
        var idStr =
            User.FindFirstValue(ClaimTypes.NameIdentifier) ??
            User.FindFirstValue(JwtRegisteredClaimNames.Sub);

        if (!int.TryParse(idStr, out var userId))
            throw new InvalidOperationException("Id người dùng trong JWT không hợp lệ.");

        return userId;
    }
}
