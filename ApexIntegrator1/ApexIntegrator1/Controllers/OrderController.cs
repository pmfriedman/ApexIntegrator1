using ApexServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ApexIntegrator1.Controllers
{
    public class OrderController : Controller
    {
        // GET: Order
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> CreateOrder(string address)
        {
            await new SessionService().DeleteCurrentSession();

            var svc = new OrderService();
            var order = await svc.CreateOrderAsync(address);
            
            return Json(order);
        }

        //public async Task<List<OrderInfo>> GetOrders()
        //{

        //}
    }
}