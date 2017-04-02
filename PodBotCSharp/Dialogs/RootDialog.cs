using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Web.Configuration;

namespace PodBotCSharp.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            // Debugging Purposes
            //await context.PostAsync("activity.Text: " + activity.Text);

            switch (activity.Text)
            {
                case "/addpost":
                    await context.PostAsync("Calling AddPostDialog");
                    context.Call(new AddPostDialog(), PostTaskCompletion);
                    break;
                default:
                    await context.PostAsync("Sorry I didn't get you.");
                    break;
            }

            context.Wait(MessageReceivedAsync);
        }

        private async Task PostTaskCompletion(IDialogContext context, IAwaitable<object> result)
        {
            context.Wait(MessageReceivedAsync);
        }
    }
}