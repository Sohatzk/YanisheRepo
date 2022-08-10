using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using NewYanishe.Entities;

namespace NewYanishe
{
    public class MessageContext : DbContext
    {
        public DbSet<Message> Messages { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=.;Database=yanishedb;Trusted_Connection=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Message>()
                .HasIndex(m => m.Text)
                .HasName("IX_Text");

            var chatMessages = ChatParser.ReadMessages(
                @"C:\Users\sohat\Downloads\Telegram Desktop\ChatExport_2022-08-08\result.json",
                "user983461650",
                "user372102354"
                );
            modelBuilder.Entity<Message>().HasData(chatMessages);
        }
    }
}
