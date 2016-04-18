using ApexServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Runtime.Serialization;
using ApexServices.Apex;

namespace ApexIntegrator1.Controllers
{
    public class GeocodeController : Controller
    {
        // GET: Geocode
        public ActionResult Index(string result)
        {
            return View(result);
        }

        [HttpPost]
        public async Task<ActionResult> DoGeocode(string text)
        {
            var service = new GeocodeService();
            var geocode = await service.GeocodeAsync(text);

            return Json(geocode?.ResultsData?[0]?.Description);
        }
    }
}