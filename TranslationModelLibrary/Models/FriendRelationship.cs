using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslationModelLibrary.Models
{
    public class FriendRelationship
    {
        public int Id { get; set; }
        public string UserId1 { get; set; }
        public string UserId2 { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
