using Chat.Models;
using Firebase.Database;
using Firebase.Database.Query;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Chat.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private FirebaseClient firebaseClient;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            firebaseClient = new FirebaseClient("https://brightideaschat.firebaseio.com/");
        }

        public async Task<ActionResult> About()
        {
            //Simulate test user data and login timestamp
            var userId = "12345";
            //var currentLoginTime = DateTime.UtcNow.ToString("MM/dd/yyyy HH:mm:ss");

            //Save non identifying data to Firebase
            var currentUserLogin = new MessageData("Joe Test");
            var result = await firebaseClient
              .Child("Messages")
              .PostAsync(currentUserLogin);

            //Retrieve data from Firebase
            var dbLogins = await firebaseClient
              .Child("Messages")
              .OnceAsync<MessageData>();

            List<string> timestampList = new List<string>();

            //Convert JSON data to original datatype
            foreach (var login in dbLogins)
            {
                timestampList.Add(login.Object.Message);
            }

            //Pass data to the view
            ViewBag.CurrentUser = userId;
            ViewBag.Logins = timestampList.OrderByDescending(x => x);
            return View();
        }
        public async Task<IActionResult> send()
        {
            IFormCollection form = await Request.ReadFormAsync();
            string message = form["message"];
            // now we have the message from the form!
            FirebaseObject<MessageData> result = await firebaseClient.Child("Messages").PostAsync(new MessageData(message));
            return View("~/Views/Home/Index.cshtml");
        }

        public IActionResult Index()
        {
            return View();
        }

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
