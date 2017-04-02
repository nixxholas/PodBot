using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Web.Configuration;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace PodBotCSharp.Dialogs
{
    [Serializable]
    public class AddPostDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            // Debugging Purposes
            //await context.PostAsync("Running AddPostDialog");

            // Handling conversational information
            // https://www.youtube.com/watch?v=TyrpJBM3nJU
            await context.PostAsync("Can I have the URL to the post?");

            context.Wait(MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            // Debugging Purposes
            await context.PostAsync("ServiceUrl: " + activity.ServiceUrl);

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
            HeroCard plCard = new HeroCard()
            {
                Title = (string) jObject["title"],
                Images = cardImages,
                Buttons = cardButtons
            };

            // Create the message
            // https://github.com/Microsoft/BotBuilder-Samples/tree/master/CSharp/cards-RichCards
            var message = context.MakeMessage();

            // Add the card to the message as an attachment
            message.Attachments.Add(plCard.ToAttachment());

            // Show the user a preview and post the data to the telegram channel
            await context.PostAsync(message);

            // Telegram Hook Test
            //await WebApiConfig.TelegramHook.SendTextMessageAsync("@HypeThePod", "Hello teLEGRAM");

            BroadcastCardToTelegramChannel(plCard);
            
            context.Done(new object());
        }

        private static async void BroadcastCardToTelegramChannel(HeroCard cardAttachment)
        {
            // Send the image,
            await WebApiConfig.TelegramHook.SendPhotoAsync(WebConfigurationManager.AppSettings["TelegramChannelId"],
                cardAttachment.Images[0].Url, cardAttachment.Title);

            // Then send the actions/buttons
            //await WebApiConfig.TelegramHook.Send
        }
    }
}