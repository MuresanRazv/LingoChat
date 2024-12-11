using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslationModelLibrary.Models
{
    public class TranslatedMessage
    {
        public int Id { get; set; }
        public string SourceLanguage { get; set; }
        public string TargetLanguage { get; set; }
        public int MessageId { get; set; }
        public string TranslatedContent { get; set; }
    }
}
