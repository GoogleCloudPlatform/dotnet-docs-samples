using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TranslatorUI.Models;

namespace TranslatorUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly FirestoreDb _firestore;
        private CollectionReference _translations;

        public HomeController(FirestoreDb firestore)
        {
            _firestore = firestore;
            _translations = _firestore.Collection("Translations");
        }

        public async Task<IActionResult> Index()
        {
            var query = _translations.OrderByDescending("TimeStamp")
                .Limit(20);
            var snapshot = await query.GetSnapshotAsync();
            var model = new HomeViewModel() 
            {
                Translations = snapshot.Documents.Select(
                    doc => doc.ConvertTo<Translation>()).ToList()
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
