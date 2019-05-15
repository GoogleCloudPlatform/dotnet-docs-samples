using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TranslatorUI.Models;
using Google.Cloud.PubSub.V1;
using Google.Protobuf;

namespace TranslatorUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly FirestoreDb _firestore;
        private readonly PublisherClient _publisher;
        private CollectionReference _translations;

        public HomeController(FirestoreDb firestore, PublisherClient publisher)
        {
            _firestore = firestore;
            _publisher = publisher;
            _translations = _firestore.Collection("Translations");
        }

        [HttpPost]
        [HttpGet]
        public async Task<IActionResult> Index(string SourceText)
        {
            // Look up the most recent 20 translations.
            var query = _translations.OrderByDescending("TimeStamp")
                .Limit(20);
            var snapshotTask = query.GetSnapshotAsync();

            if (!string.IsNullOrWhiteSpace(SourceText)) 
            {
                // Submit a new translation request.
                await _publisher.PublishAsync(new PubsubMessage()
                {
                    Data = ByteString.CopyFromUtf8(SourceText)
                });
            }

            // Render the page.
            var model = new HomeViewModel() 
            {
                Translations = (await snapshotTask).Documents.Select(
                    doc => doc.ConvertTo<Translation>()).ToList(),
                SourceText = SourceText
            };
            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
