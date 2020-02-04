using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Chat.Models;
using Firebase.Database;
using Firebase.Database.Query;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Chat.Tests
{
    [TestFixture]
    public class TestChat
    {

        private FirebaseClient client;

        [SetUp]
        public void Setup()
        {
            client = new FirebaseClient("https://brightideaschat.firebaseio.com/");
        }

        [Test]
        public async Task TestSendMsgToDatabase()
        {
            Random rng = new Random();
            int randValue = rng.Next(10000000, 99999999);
            string message = $"Test Message_{randValue}";
                
            FirebaseObject<string> result = await client.Child("Messages")
                .PostAsync($"{{\"Message\": \"{message}\"}}");
            var database = await client
                .Child("Messages")
                .OnceAsync<MessageData>();
            
            List<string> messages = new List<string>();
            foreach (FirebaseObject<MessageData> msg in database)
            {
                messages.Add(msg.Object.Message);
            }

            if (messages.Contains(message))
            {
                Assert.Pass("Message was stored in the database");
            }
            else
            {
                Assert.Fail("Message was not found in the database");
            }
            
            // clean up by deleting the message
            await client.Child("Messages").Child(message).DeleteAsync();
        }
        
    }
}