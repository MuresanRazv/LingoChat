using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TranslationModelLibrary.Models;

namespace TranslationModelLibrary.Context
{
    public class TranslationContext: DbContext
    {
        public TranslationContext(DbContextOptions<TranslationContext> options)
            : base(options)
        {
        }

        public DbSet<Message> Messages { get; set; }
        public DbSet<TranslatedMessage> TranslatedMessages { get; set; }
        public DbSet<FriendRequests> FriendRequests { get; set; }
        public DbSet<FriendRelationship> FriendRelationships { get; set; }
    }
}
