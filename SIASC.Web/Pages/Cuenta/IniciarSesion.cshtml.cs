using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using SIASC.Infrastructure.Identity;

namespace SIASC.Web.Pages.Cuenta;

[AllowAnonymous]
public class IniciarSesionModel : PageModel
{
    private readonly SignInManager<UsuarioSistema> _signInManager;
    private readonly UserManager<UsuarioSistema> _userManager;

    public IniciarSesionModel(
        SignInManager<UsuarioSistema> signInManager,
        UserManager<UsuarioSistema> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    [BindProperty]
    public DatosAcceso Input { get; set; } = new();

    public string? ReturnUrl { get; set; }

    public class DatosAcceso
    {
        [Required(ErrorMessage = "Ingresa tu correo electrónico.")]
        [EmailAddress(ErrorMessage = "Ingresa un correo válido.")]
        [Display(Name = "Correo electrónico")]
        public string CorreoElectronico { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ingresa tu contraseña.")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Contrasena { get; set; } = string.Empty;
    }

    public IActionResult OnGet(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return LocalRedirect(Url.Content("~/"));
        }

        ReturnUrl = returnUrl;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(
        string? returnUrl = null)
    {
        ReturnUrl = returnUrl;

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var usuario = await _userManager.FindByEmailAsync(
            Input.CorreoElectronico.Trim());

        if (usuario == null || !usuario.Activo)
        {
            ModelState.AddModelError(
                string.Empty,
                "Correo electrónico o contraseña incorrectos.");

            return Page();
        }

        var resultado = await _signInManager.PasswordSignInAsync(
            usuario,
            Input.Contrasena,
            isPersistent: false,
            lockoutOnFailure: true);

        if (resultado.IsLockedOut)
        {
            ModelState.AddModelError(
                string.Empty,
                "La cuenta está bloqueada temporalmente. Intenta más tarde.");

            return Page();
        }

        if (!resultado.Succeeded)
        {
            ModelState.AddModelError(
                string.Empty,
                "Correo electrónico o contraseña incorrectos.");

            return Page();
        }

        if (!string.IsNullOrWhiteSpace(returnUrl)
            && Url.IsLocalUrl(returnUrl))
        {
            return LocalRedirect(returnUrl);
        }

        return RedirectToPage("/Index");
    }
}

