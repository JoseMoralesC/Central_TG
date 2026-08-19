using System.Collections.Generic;
using System.Web.Mvc;
using WebAdministrativa.Models;

namespace WebAdministrativa.Controllers
{
    public class LineasController : Controller
    {
        public ActionResult Nuevas()
        {
            return View(new List<Linea>());
        }

        [HttpGet]
        public ActionResult Crear()
        {
            return View(new NuevaLineaViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Crear(NuevaLineaViewModel modelo)
        {
            if (!ModelState.IsValid) return View(modelo);

            TempData["Mensaje"] = "La linea fue validada. El registro se conectara al Web Service en la siguiente etapa.";
            return RedirectToAction("Nuevas");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Eliminar(int id)
        {
            TempData["Mensaje"] = "La eliminacion se conectara al Web Service en la siguiente etapa.";
            return RedirectToAction("Nuevas");
        }

        public ActionResult Activar()
        {
            return View(new List<Linea>());
        }

        [HttpGet]
        public ActionResult ConfirmarActivacion(int id)
        {
            return View(new ActivarLineaViewModel { IdLinea = id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ConfirmarActivacion(ActivarLineaViewModel modelo)
        {
            if (!ModelState.IsValid) return View(modelo);

            TempData["Mensaje"] = "La activacion se conectara al Web Service en la siguiente etapa.";
            return RedirectToAction("Activar");
        }

        public ActionResult Desactivar()
        {
            return View(new List<Linea>());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ConfirmarDesactivacion(int id)
        {
            TempData["Mensaje"] = "La desactivacion se conectara al Web Service en la siguiente etapa.";
            return RedirectToAction("Desactivar");
        }
    }
}
