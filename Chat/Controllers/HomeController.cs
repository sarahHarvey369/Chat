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
        // This is a comment to test jenkins
        private readonly ILogger<HomeController> _logger;
        private FirebaseClient firebaseClient;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            firebaseClient = new FirebaseClient("https://brightideaschat.firebaseio.com/");
        }


        [HttpPost]
        public async Task<IActionResult> Send()
        {
            //Send data to Firebase
            IFormCollection form = await Request.ReadFormAsync();
            string message = form["message"];
            if (!message.Equals(""))
            {
                FirebaseObject<MessageData> result = await firebaseClient.Child("Messages").PostAsync(new MessageData(message));
            }
            var index = await Index();
            return View("~/Views/Home/Index.cshtml");
        }

        public async Task<IActionResult> Index()
        {   
            //Retrieve data from Firebase
            var database = await firebaseClient
              .Child("Messages")
              .OnceAsync<MessageData>();

            var messageList = new List<string>();
            foreach (var mess in database)
            {
                messageList.Add(mess.Object.Message);
            }
            ViewBag.Message = messageList;

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
