using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Web.Configuration;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Net;
using NetTelegramBotApi.Requests;
using NetTelegramBotApi.Types;
using InstaSharp;

namespace PodBotCSharp.Dialogs.Posts
{
    [Serializable]
    public class SetProfileDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            // Debugging Purposes
            //await context.PostAsync("Running SetProfileDialog");

            // Create a scope that define what we're gonna use
            var scopes = new List<OAuth.Scope>();
            scopes.Add(OAuth.Scope.Basic);
            scopes.Add(OAuth.Scope.Likes);
            scopes.Add(OAuth.Scope.Comments);

            var link = OAuth.AuthLink(WebConfigurationManager.AppSettings["InstagramOAuthURL"] + "authorize", WebConfigurationManager.AppSettings["InstagramClientId"]
                , WebConfigurationManager.AppSettings["InstagramRedirectUri"], scopes, OAuth.ResponseType.Code);

            context.Wait(MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            // Debugging Purposes
            //await context.PostAsync("ServiceUrl: " + activity.ServiceUrl);

            // We need to run a url check but i'll ignore that for now

            // Call Instagram's oembed API to retrieve post metadata
            string OembedAPIURL = "https://api.instagram.com/oembed?url=" + activity.Text;

            // JObject Initialization
            JObject jObject = new JObject();

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response =
                        client.GetAsync(OembedAPIURL).Result;

                if (response.IsSuccessStatusCode)
                {
                    // Retrieve the result of the response
                    var msg = response.Content.ReadAsStringAsync().Result;

                    jObject = JObject.Parse(msg);
                }
                else
                {
                    await context.PostAsync("Please enter a valid url.");

                    // Loop this method
                    context.Wait(MessageReceivedAsync);
                    return;
                }
            }

            // Successful Data Retrieval
            await context.PostAsync("Okay! I'll broadcast your post for you!");

            // Create the card images
            List<CardImage> cardImages = new List<CardImage>();
            cardImages.Add(new CardImage(url: (string)jObject["thumbnail_url"]));

            // Create the action buttons for the card first
            List<CardAction> cardButtons = new List<CardAction>();

            CardAction ViewPostButton = new CardAction()
            {
                Value = activity.Text,
                Type = "openUrl",
                Title = "View Post"
            };
            CardAction ProfileButton = new CardAction()
            {
                Value = (string)jObject["author_url"],
                Type = "openUrl",
                Title = "User Profile"
            };

            cardButtons.Add(ViewPostButton);
            cardButtons.Add(ProfileButton);

            // Create the card
            HeroCard cardAttachment = new HeroCard()
            {
                Title = (string)jObject["title"],
                Images = cardImages,
                Buttons = cardButtons
            };

            // Create the message
            // https://github.com/Microsoft/BotBuilder-Samples/tree/master/CSharp/cards-RichCards
            var message = context.MakeMessage();

            // http://stackoverflow.com/questions/40008126/how-to-set-channeldata-for-a-custom-message-in-telegram
            var sendMessageObject = JObject.FromObject(new
            {
                chat_id = WebConfigurationManager.AppSettings["TelegramChannelId"],
                text = cardAttachment.Title,
                reply_markup = new
                {
                    inline_keyboard = new dynamic[]
                    {
                                    new {
                                        text = "View Post",
                                        url = activity.Text
                                    },
                                    new {
                                        content_type = "View Profile",
                                        url = (string)jObject["author_url"]
                                    }
                    },
                }
            });

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
                            Text = "View Post",
                            Url = activity.Text
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
            var reqAction = new SendPhoto(WebConfigurationManager.AppSettings["TelegramChannelId"], new FileToSend(activity.Text))
            {   Caption = "Photo by " + (string)jObject["author_name"],
                ReplyMarkup = keyb };
            
            // Send it to via the Telegram Hook
            WebApiConfig.TelegramHook.MakeRequestAsync(reqAction).Wait(); 

            context.Done(new object());
        }
    }
}