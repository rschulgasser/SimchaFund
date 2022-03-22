using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SimchaCont.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using SimchaContributions.data;
using SimchaCont.web.Models;

namespace SimchaCont.Controllers
{
    public class HomeController : Controller
    {
        DBManager dB = new(@"Data Source=.\sqlexpress;Initial Catalog=SimchaContributions;Integrated Security=true;");
    
        public IActionResult Index()
        {
            HomeModel homeModel = new();
            homeModel.Simchos = dB.GetSimchas();

            return View(homeModel); ;
        }
        [HttpPost]
        public IActionResult NewSimcha(Simcha simcha)
        {
            dB.AddSimcha(simcha);

            return Redirect("/");
        }
        
        public IActionResult Contributions(int simchaid)
        {
            ContributionsModel c = new();
            c.Contributers = dB.GetContrirutionsforaSimcha(simchaid);
            c.Counter = 0;
            c.Simcha = dB.GetSimchById(simchaid);
            return View(c);
        }
       
            [HttpPost]
        public IActionResult UpdateContributions(List<Contributer> contributors,int SimchaId)
        {

            dB.UpdateContributions(contributors, SimchaId);
            return Redirect("/");
        }

    }
}
