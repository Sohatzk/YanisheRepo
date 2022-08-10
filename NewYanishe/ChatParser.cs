using Newtonsoft.Json.Linq;
using NewYanishe.Entities;

namespace NewYanishe
{
    public static class ChatParser
    {
        public static List<Message> ReadMessages(string fileName, params string[] ids)
        {
            var text = File.ReadAllText(fileName);

            var jObject = JObject.Parse(text);

            var allMessages = jObject["messages"];

            var messagesByUsers = new List<Message>();

            foreach (var msg in allMessages)
            {
                if (((JObject)msg).TryGetValue("from_id", out JToken fromIdToken))
                {
                    if (fromIdToken != null)
                    {
                        var value = ((JValue)fromIdToken).Value.ToString();
                        var idMatch = false;
                        foreach (var id in ids)
                        {
                            if (id == value)
                                idMatch = true;
                        }
                        if (!string.IsNullOrEmpty(value) && idMatch)
                        {
                            try
                            {
                                var message = new Message()
                                {
                                    Id = messagesByUsers.Any() ? messagesByUsers.Count() + 1 : 1,
                                    //UserId = ((JValue)((JObject)msg).GetValue("from_id")).Value.ToString(),
                                    Text = ((JValue)((JObject)msg).GetValue("text")).Value.ToString()
                                };
                                messagesByUsers.Add(message);
                            }
                            catch
                            {
                                continue;
                            }
                        }
                    }
                }
            }

            return messagesByUsers;
        }
    }
}
