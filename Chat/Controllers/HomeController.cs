using System;
using Chat.Models;
using Firebase.Database;
using Firebase.Database.Query;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting.Internal;
using Newtonsoft.Json.Linq;

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

        [Route("/home/sendajax")]
        public async Task<IActionResult> SendAjax()
        {
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                string content = await reader.ReadToEndAsync();
                JObject jsonContent = JObject.Parse(content);
                string msg = jsonContent["message"].ToString();
                FirebaseObject<MessageData> result = await firebaseClient
                    .Child("Messages")
                    .PostAsync(new MessageData(msg));
            }
            
            return new JsonResult(Ok());
        }
        
        [Route("/home/getajax")]
        public async Task<IActionResult> GetAjax()
        {
            var database = await firebaseClient
                .Child("Messages")
                .OnceAsync<MessageData>();
            
            JObject messagesObj = new JObject();
            JArray messagesList = new JArray();
            messagesObj["messages"] = messagesList;
            foreach (FirebaseObject<MessageData> msg in database)
            {
                messagesList.Add(msg.Object.Message);
            }

            Response.ContentType = "application/json";
            return Content(messagesObj.ToString());
        }

        public async Task<IActionResult> Index()
        {
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
