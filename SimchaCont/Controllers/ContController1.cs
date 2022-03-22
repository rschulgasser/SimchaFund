using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SimchaContributions.data;
using SimchaCont.web.Models;

namespace SimchaCont.web.Controllers
{
    public class ContController1 : Controller
    {
        DBManager dB = new(@"Data Source=.\sqlexpress;Initial Catalog=SimchaContributions;Integrated Security=true;");
        public IActionResult IndexContributers()
        {
            ContributerModel c = new();
            c.Contributers = dB.GetAllContributers();
            c.Total = dB.GetTotal();

            return View(c);
        }
        public IActionResult History(int ContributerId)
        {
            HistoryModel h = new();
            h.HistoryItems = dB.GetHistoryItems(ContributerId);
            h.Total = dB.GetPersonsBalance(ContributerId);

            return View(h);
        }
        [HttpPost]
        public IActionResult New(Contributer contributer,int initialDeposit)
        {
           contributer.CreatedDate = DateTime.Now;
            //DateTime d=contributer.CreatedDate;
            dB.AddContributer(contributer, initialDeposit);

            return Redirect("/ContController1/IndexContributers");
        }
        public IActionResult Deposit(int contributorId, DateTime date, int amount)
        {
            dB.AddDeposit(contributorId, amount, date);

            return Redirect("/ContController1/IndexContributers");
        }

    }
}
