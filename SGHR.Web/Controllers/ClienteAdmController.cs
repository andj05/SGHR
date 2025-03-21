using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SGHR.Web.Controllers
{
    public class ClienteAdmController : Controller
    {
        // GET: ClienteAdmController
        public ActionResult Index()
        {
            return View();
        }

        // GET: ClienteAdmController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ClienteAdmController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ClienteAdmController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ClienteAdmController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ClienteAdmController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ClienteAdmController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ClienteAdmController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
