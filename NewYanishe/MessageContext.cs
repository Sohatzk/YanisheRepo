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
            var chatMessages = ReadMessages(@"C:\Users\sohat\Downloads\Telegram Desktop\ChatExport_2022-08-08\result.json");

            modelBuilder.Entity<Message>().HasData(chatMessages);
        }

        public static List<Message> ReadMessages(string fileName)
        {
            var text = File.ReadAllText(fileName);

            var jObject = JObject.Parse(text);

            var messages = jObject["messages"];

            var yanMessages = new List<Message>();

            foreach (var msg in messages)
            {
                if (((JObject)msg).TryGetValue("from_id", out JToken fromIdToken))
                {
                    if (fromIdToken != null)
                    {
                        var value = ((JValue)fromIdToken).Value.ToString();
                        if (!string.IsNullOrEmpty(value)
                            && (value == "user983461650" || value == "user372102354"))
                        {
                            try
                            {
                                var message = new Message()
                                {
                                    Id = yanMessages.Any() ? yanMessages.Count() + 1 : 1,
                                    //UserId = ((JValue)((JObject)msg).GetValue("from_id")).Value.ToString(),
                                    Text = ((JValue)((JObject)msg).GetValue("text")).Value.ToString()
                                };
                                yanMessages.Add(message);
                            }
                            catch
                            {
                                continue;
                            }
                        }
                    }
                }
            }

            return yanMessages;
        }
    }
}
