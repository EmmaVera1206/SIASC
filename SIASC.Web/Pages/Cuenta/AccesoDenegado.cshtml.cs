
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SIASC.Web.Pages.Cuenta;

[AllowAnonymous]
public class AccesoDenegadoModel : PageModel
{
    public void OnGet()
    {
    }
}

