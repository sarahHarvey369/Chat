using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Chat.Models;
using Firebase.Database;
using Firebase.Database.Query;

namespace Chat.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<ActionResult> Index()
        {
            //Simulate test user data and login timestamp
            var message = "say it!";
            //var currentLoginTime = DateTime.UtcNow.ToString("MM/dd/yyyy HH:mm:ss");

            //Save non identifying data to Firebase
            var currentMessage = new LoginData() { Message = message };
            var firebaseClient = new FirebaseClient("https://brightideaschat.firebaseio.com/");
            var result = await firebaseClient
              .Child("Messages/")
              .PostAsync(currentMessage);

            //Retrieve data from Firebase
            var dbLogins = await firebaseClient
              .Child("Messages")
              //.Child(message)
              .OnceAsync<LoginData>();

            //var messageList = new List<String>();

            ////Convert JSON data to original datatype
            //foreach (var login in dbLogins)
            //{
            //    messageList.Add(login.Object.Message);
            //}

            ////Pass data to the view
            //ViewBag.CurrentMessage = message;
            //ViewBag.Messages = messageList.OrderByDescending(x => x);
            return View();
        }

        
        //public IActionResult ChatSubmit()
        //{

        //}


        //public IActionResult Index()
        //{
        //    return View();
        //}

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
