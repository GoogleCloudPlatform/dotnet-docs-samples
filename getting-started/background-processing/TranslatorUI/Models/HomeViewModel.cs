using System.Collections.Generic;

namespace TranslatorUI.Models
{
    public class HomeViewModel 
    { 
        public List<Translation> Translations { get; set; } = new List<Translation>();
    }
}