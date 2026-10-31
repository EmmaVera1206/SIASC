using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SIASC.Infrastructure.Identity;

namespace SIASC.Web.Pages.Cuenta;

[Authorize]
public class CerrarSesionModel : PageModel
{
    private readonly SignInManager<UsuarioSistema> _signInManager;

    public CerrarSesionModel(
        SignInManager<UsuarioSistema> signInManager)
    {
        _signInManager = signInManager;
    }

    public IActionResult OnGet()
    {
        return RedirectToPage("/Index");
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await _signInManager.SignOutAsync();

        return RedirectToPage("/Cuenta/IniciarSesion");
    }
}

