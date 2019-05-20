using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace TranslateWorker.Controllers
{
    public class PubsubMessage {
        public string data { get; set; }
        public string messageId { get; set; }
        public Dictionary<string, string> attributes { get; set; }
    }

    public class PostMessage {
        public PubsubMessage message { get; set; }
        public string subscription { get; set; }        
    }


    [Route("api/[controller]")]
    [ApiController]
    public class TranslateController : ControllerBase
    {
        private readonly ILogger<TranslateController> _logger;
        private readonly FirestoreDb _firestore;
        private readonly Google.Cloud.Translation.V2.TranslationClient _translator;
        private readonly CollectionReference _translations;

        public TranslateController(ILogger<TranslateController> logger,
            FirestoreDb firestore,
            Google.Cloud.Translation.V2.TranslationClient translator)
        {
            this._logger = logger;
            this._firestore = firestore;
            this._translator = translator;
            _translations = _firestore.Collection("Translations");
        }

        // POST api/translate
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] string value)
        {
            // Unpack the message from Pubsub.
            string sourceText;
            try {
                PostMessage request = JsonConvert
                    .DeserializeObject<PostMessage>(value);
                byte[] data = Convert.FromBase64String(request.message.data);
                sourceText = Encoding.UTF8.GetString(data);
            } catch (Exception e) {
                _logger.LogError(1, e, "Bad request");
                return BadRequest();
            }
            // Translate the source text.
            var result = await _translator.TranslateTextAsync(sourceText, "es");
            // Store the result in Firestore.
            Translation translation = new Translation() 
            {
                TimeStamp = DateTime.UtcNow,
                SourceText = sourceText,
                TranslatedText = result.TranslatedText                
            };
            await _translations.AddAsync(translation);
            // Return a success code.
            return NoContent();
        }

        [Route("/")]
        public IActionResult Index()
        {
            return Content("Serving translate requests...");
        }
    }
}
