using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Collections.Generic;
using InstaSharp.Models.Responses;
using System.Web.Configuration;
using Newtonsoft.Json.Linq;
using NetTelegramBotApi.Types;
using NetTelegramBotApi.Requests;

namespace PodBotCSharp.Dialogs.User
{
    [Serializable]
    public class ShareIGProfileDialog : IDialog<object>
    {
        private BotData _botData { get; set; }
        // Default Constructor for now
        public ShareIGProfileDialog(BotData userActivity) {
            _botData = userActivity;
            
        }
    
        public async Task StartAsync(IDialogContext context)
        {
            OAuthResponse oauth = _botData.GetProperty<OAuthResponse>("InstaSharp.OAuth");

            // Successful Data Retrieval
            await context.PostAsync("Okay! I'll share your profile for you!");

            // Create the card images
            List<CardImage> cardImages = new List<CardImage>();
            cardImages.Add(new CardImage(url: oauth.User.ProfilePicture));

            // Create the action buttons for the card first
            List<CardAction> cardButtons = new List<CardAction>();

            CardAction ViewPostButton = new CardAction()
            {
                Value = "http://instagram.com/" + oauth.User.Username,
                Type = "openUrl",
                Title = "View Profile"
            };
            cardButtons.Add(ViewPostButton);

            // Create the card
            HeroCard cardAttachment = new HeroCard()
            {
                Title = oauth.User.FullName,
                Images = cardImages,
                Buttons = cardButtons
            };

            // Create the message
            // https://github.com/Microsoft/BotBuilder-Samples/tree/master/CSharp/cards-RichCards
            var message = context.MakeMessage();
            
            // Add the card to the message as an attachment
            message.Attachments.Add(cardAttachment.ToAttachment());

            // Show the user a preview and post the data to the telegram channel
            await context.PostAsync(message);

            // Telegram Hook Test

            // First, create the buttons first
            var keyb = new InlineKeyboardMarkup()
            {
                InlineKeyboard = new InlineKeyboardButton[][]
                {
                    new[] {
                        new InlineKeyboardButton() {
                            Text = "View Profile",
                            Url = "http://instagram.com/" + oauth.User.Username
                        }
                    },
                    //new[]
                    //{
                    //    new InlineKeyboardButton()
                    //    {
                    //        Text = "View User's Profile",
                    //        Url = (string)jObject["author_url"],
                    //    }
                    //}
                }
            };

            // Now, we create the actual message
            var reqAction = new SendPhoto(WebConfigurationManager.AppSettings["TelegramTestChannelId"], 
                new FileToSend(oauth.User.ProfilePicture))
            {
                Caption = oauth.User.FullName,
                ReplyMarkup = keyb
            };

            // Send it to via the Telegram Hook
            WebApiConfig.TelegramHook.MakeRequestAsync(reqAction).Wait();


            context.Done(new object());
        }
    }
}