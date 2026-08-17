using System.Web.Mvc;
using WebAdministrativa.Models;

namespace WebAdministrativa.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public ActionResult Login()
        {
            return View(new Usuario());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Usuario usuario)
        {
            if (!ModelState.IsValid)
            {
                return View(usuario);
            }

            // TODO: Cifrar datos e invocar WS_AUTENTICACION1.
            ModelState.AddModelError("", "La autenticacion se conectara al Web Service en la siguiente etapa.");
            return View(usuario);
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
