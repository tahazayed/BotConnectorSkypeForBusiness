using Microsoft.Bot.Connector.Emulator;
using Microsoft.Lync.Model;
using Microsoft.Lync.Model.Conversation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace MrSamwiseBot
{
    public partial class MainForm : Form
    {
        private LyncClient _lyncClient;
        private ConversationManager _conversationManager;
        private readonly ConnectorEmulator oConnectorEmulator = new ConnectorEmulator();



        string response=string.Empty;

        public MainForm()
        {
            InitializeComponent();

            _lyncClient = LyncClient.GetClient();
            _conversationManager = _lyncClient.ConversationManager;
            _conversationManager.ConversationAdded += ConversationAdded;
            try
            {
                oConnectorEmulator.Port = Properties.Settings.Default.ConnectorEmulatorPort;
                oConnectorEmulator.StartServer();
            }
            catch
            {
                MessageBox.Show(string.Format("I can not start as Port:{0} is used by another application", Properties.Settings.Default.ConnectorEmulatorPort));
            }
  
            
        }

        private void ConversationAdded(object sender, ConversationManagerEventArgs e)
        {
            var conversation = e.Conversation;
            conversation.ParticipantAdded += ParticipantAdded;
        }

        private void ParticipantAdded(object sender, ParticipantCollectionChangedEventArgs e)
        {
            var participant = e.Participant;
            if (participant.IsSelf)
            {
                return;
            }

            var instantMessageModality =
                e.Participant.Modalities[ModalityTypes.InstantMessage] as InstantMessageModality;
            instantMessageModality.InstantMessageReceived += InstantMessageReceived;
        }

        private async void InstantMessageReceived(object sender, MessageSentEventArgs e)
        {
            var text = e.Text.Replace(Environment.NewLine, string.Empty);
            string myRemoteParticipantUri = (sender as InstantMessageModality).Endpoint.Uri.Replace("sip:", string.Empty);
            LogMessage(myRemoteParticipantUri, text);
            try
            {
                ConversationModel oConversationModel = new ConversationModel(oConnectorEmulator,
                    Properties.Settings.Default.BotURL,
                    Properties.Settings.Default.AppId,
                    Properties.Settings.Default.AppSecret,
                    myRemoteParticipantUri,
                    myRemoteParticipantUri,
                    "en-US");

                var oMessage = oConversationModel.CreateMessage(text, "Message");
                var oResponse = await oConversationModel.SendMessageAsync(oMessage);
                BotResult oBotResult = new BotResult();
                oBotResult = JsonConvert.DeserializeObject<BotResult>(oResponse.Content);

                string strResult = oBotResult.text;
                foreach (var att in oBotResult.attachments)
                {
                    strResult += Environment.NewLine;
                    foreach (var act in att.actions)
                    {
                        strResult += "   " + act.title + Environment.NewLine;
                    }
                }
            (sender as InstantMessageModality).BeginSendMessage(strResult, null, null);
            }
            catch(Exception ex)
            {
                LogMessage(myRemoteParticipantUri, ex.Message);
            }
        }

       
        void StartConversation(string myRemoteParticipantUri, string MSG)
        {
           
            foreach (var con in _conversationManager.Conversations)
            {
                if (con.Participants.Where(p => p.Contact.Uri == "sip:" + myRemoteParticipantUri).Count() > 0)
                {
                    if (con.Participants.Count == 2)
                    {
                        con.End();
                        break;
                    }
                }
            }

            Conversation _Conversation = _conversationManager.AddConversation();
            _Conversation.AddParticipant(_lyncClient.ContactManager.GetContactByUri(myRemoteParticipantUri));


            Dictionary<InstantMessageContentType, String> messages = new Dictionary<InstantMessageContentType, String>();
            messages.Add(InstantMessageContentType.PlainText, MSG);
            InstantMessageModality m = (InstantMessageModality)_Conversation.Modalities[ModalityTypes.InstantMessage];
            m.BeginSendMessage(messages, null, messages);
            LogMessage(myRemoteParticipantUri, MSG);
        }

        private static void LogMessage(string myRemoteParticipantUri, string MSG)
        {
            
        }

    }
}
