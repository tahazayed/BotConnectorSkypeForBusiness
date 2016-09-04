using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MrSamwiseBot
{

    public class Action
    {
        public string title { get; set; }
        public string message { get; set; }
    }

    public class Attachment
    {
        public List<Action> actions { get; set; }
    }

    public class From
    {
        public string name { get; set; }
        public string channelId { get; set; }
        public string address { get; set; }
        public bool isBot { get; set; }
    }

    public class To
    {
        public string name { get; set; }
        public string channelId { get; set; }
        public string address { get; set; }
        public bool isBot { get; set; }
    }

    public class Participant
    {
        public string name { get; set; }
        public string channelId { get; set; }
        public string address { get; set; }
    }

    public class BotUserData
    {
    }

    public class BotConversationData
    {
    }

    public class BotPerUserInConversationData
    {
        public string DialogState { get; set; }
    }

    public class BotResult
    {
        public string conversationId { get; set; }
        public string language { get; set; }
        public string text { get; set; }
        public List<Attachment> attachments { get; set; }
        public From from { get; set; }
        public To to { get; set; }
        public string replyToMessageId { get; set; }
        public List<Participant> participants { get; set; }
        public int totalParticipants { get; set; }
        public string channelMessageId { get; set; }
        public string channelConversationId { get; set; }
        public BotUserData botUserData { get; set; }
        public BotConversationData botConversationData { get; set; }
        public BotPerUserInConversationData botPerUserInConversationData { get; set; }
    }
}
