﻿using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using PodBotCSharp.Dialogs.Posts;
using System.Web.Configuration;
using PodBotCSharp.Dialogs.User;
using System.Collections.Generic;
using System.Web;

namespace PodBotCSharp.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            // Get access token from bot state
            ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
            StateClient stateClient = activity.GetStateClient();
            BotState botState = new BotState(stateClient);
            BotData botData = await botState.GetUserDataAsync(activity.ChannelId, activity.From.Id);

            // Debugging Purposes
            //await context.PostAsync("activity.From.Id: " + activity.From.Name + " | activity.From.Name: " + activity.From.Name);

            string token = botData.GetProperty<string>("igAccessToken");

            // Debugging Purposes
            await context.PostAsync(token);

            // if the does not have the token
            if (token == null)
            {
                // Add him to the userbase list if not already
                if (!WebApiConfig.UserBase.ContainsKey(activity.From.Id))
                {
                    WebApiConfig.UserBase.Add(activity.From.Id, new Models.UserChannelData()
                    {
                        ChannelId = activity.ChannelId
                    });
                }

                Activity replyToConversation = activity.CreateReply();
                replyToConversation.Recipient = activity.From;
                replyToConversation.Type = "message";

                replyToConversation.Attachments = new List<Attachment>();
                List<CardAction> cardButtons = new List<CardAction>();
                CardAction plButton = new CardAction()
                {
                    Value = $"{System.Configuration.ConfigurationManager.AppSettings["InstagramLocalOAuthUri"]}/?id=" + activity.From.Id,
                    Type = "signin",
                    Title = "Allow Access"
                };
                cardButtons.Add(plButton);
                SigninCard plCard = new SigninCard("Please Authorize Instagram Access", new List<CardAction>() { plButton });
                Attachment plAttachment = plCard.ToAttachment();
                replyToConversation.Attachments.Add(plAttachment);

                var reply = await connector.Conversations.SendToConversationAsync(replyToConversation);
            }
            else
            {

                if (!activity.From.Name.Equals(WebConfigurationManager.AppSettings["TelegramTestChannelId"].TrimStart('@'))
                    // http://stackoverflow.com/questions/3222125/fastest-way-to-remove-first-char-in-a-string
                    && !activity.From.Name.Equals(WebConfigurationManager.AppSettings["TelegramChannelId"].TrimStart('@')))
                {
                    switch (activity.Text)
                    {
                        case "/addpost":
                            //await context.PostAsync("Calling AddPostDialog");
                            context.Call(new AddPostDialog(), PostTaskCompletion);
                            break;
                        case "/setprofile":
                            context.Call(new SetIGProfileDialog(botData), PostTaskCompletion);
                            break;
                        default:
                            await context.PostAsync("Sorry I didn't get you.");
                            context.Wait(MessageReceivedAsync);
                            break;
                    }
                }
                else
                {
                    context.Done(new object());
                }
            }
        }

        private async Task PostTaskCompletion(IDialogContext context, IAwaitable<object> result)
        {
            context.Wait(MessageReceivedAsync);
        }
    }
}