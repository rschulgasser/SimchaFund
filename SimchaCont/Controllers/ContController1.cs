using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SimchaContributions.data;
using SimchaCont.web.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace SimchaCont.web.Controllers
{
    public class ContController1 : Controller
    {
        DBManager dB = new(@"Data Source=.\sqlexpress;Initial Catalog=SimchaContributions;Integrated Security=true;");
        public IActionResult IndexContributers()
        {
            ContributerModel c = new();

            string ContributionMessage = (string)TempData["CMessage"];
            if (!String.IsNullOrEmpty(ContributionMessage))
            {
                c.Message = ContributionMessage;
            }
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
           int id= dB.AddContributer(contributer, initialDeposit);
             TempData["CMessage"] = $"Contributer added successfully: id: {id}!";

            return Redirect("/ContController1/IndexContributers");
        }
        [HttpPost]
        public IActionResult Deposit(int contributorId, DateTime date, int amount)
        {
            dB.AddDeposit(contributorId, amount, date);
            TempData["CMessage"] = $"Deposit Added Successfully!";
            return Redirect("/ContController1/IndexContributers");
        }
        [HttpPost]

        public IActionResult Edit(Contributer contributer)    
        {
            contributer.CreatedDate = DateTime.Now;
            dB.EditContributer(contributer);
           TempData["CMessage"] = $"Contributibuter edit successfully!";

            return Redirect("/ContController1/IndexContributers");
        }
    }
  
    
}
