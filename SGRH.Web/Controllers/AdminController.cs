using Microsoft.AspNetCore.Authorization;

namespace SGRH.Web.Controllers
{
    [Authorize(Roles = "Administrador")]
    public abstract class AdminController : BaseController
    {
    }
}
