
using Newtonsoft.Json.Linq;
using System;
using System.Web.Mvc;

namespace Hit.Mvc.Demo.Controllers
{
    public class IndexController : Controller
    {

        public IndexController()
        {

        }



        public ActionResult Index(aa[] model)
        {
            var d = JObject.Parse("{dd:\"2016-01-22 01:22:33\"}").ToObject<aa>();
            var dc = JObject.Parse("{dd:\"2016-01-22 01:22:33\"}")["dd"].ToObject<object>();

            return new JsonNetResult { Data = new { a = System.DateTime.Now, ss = JObject.FromObject(new { dd = System.DateTime.Now }).ToString() } };
        }

        public ActionResult v1()
        {

            return View((object)"model string");
        }




    }
    public class aa
    {
        public DateTime dd { get; set; }
        public string[] ar { get; set; }
    }





}
