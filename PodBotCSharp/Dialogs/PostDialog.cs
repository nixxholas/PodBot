﻿using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Web.Configuration;
using System.Collections.Generic;

namespace PodBotCSharp.Dialogs
{
    [Serializable]
    public class PostDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;
            var connector = new ConnectorClient(new Uri(activity.ServiceUrl));

            // Handling conversational information
            // https://www.youtube.com/watch?v=TyrpJBM3nJU
            switch (activity.Text)
            {
                case "/addpost":
                    await context.PostAsync("Can I have the URL to the post?");

                    // Head to the next method and wait for the reply
                    context.Wait(ProcessPostData);
                    break;
                default:
                    await context.PostAsync("Sorry I didn't get you.");
                    break;
            }

            context.Wait(MessageReceivedAsync);
        }

        private async Task ProcessPostData(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            // We need to run a url check but i'll ignore that for now
            
            await context.PostAsync("Okay! I'll broadcast your post for you!");

            // Create the card images
            List<CardImage> cardImages = new List<CardImage>();
            cardImages.Add(new CardImage(url: activity.Text));

            // Create the action buttons for the card first
            List<CardAction> cardButtons = new List<CardAction>();
            CardAction plButton = new CardAction()
            {
                Value = activity.Text,
                Type = "openUrl",
                Title = "Like"
            };
            cardButtons.Add(plButton);

            // Create the card
            HeroCard plCard = new HeroCard()
            {
                Title = "Instagram Post",
                Subtitle = "",
                Images = cardImages,
                Buttons = cardButtons
            };

            // Create the message
            // https://github.com/Microsoft/BotBuilder-Samples/tree/master/CSharp/cards-RichCards
            var message = context.MakeMessage();

            // Add the card to the message as an attachment
            message.Attachments.Add(plCard.ToAttachment());

            // Post the data to the telegram channel
            await context.PostAsync(message);
        }
    }
}