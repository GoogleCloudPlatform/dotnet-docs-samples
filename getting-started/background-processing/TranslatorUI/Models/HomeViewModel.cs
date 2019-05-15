using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TranslatorUI.Models
{
    public class HomeViewModel 
    { 
        public List<Translation> Translations { get; set; } = new List<Translation>();

        [Required]
        public string SourceText { get; set; }
    }
}