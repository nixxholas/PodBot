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
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            // Debugging Purposes
            //await context.PostAsync("activity.Text: " + activity.Text);

            switch (activity.Text)
            {
                case "/addpost":
                    //await context.PostAsync("Calling AddPostDialog");
                    context.Call(new AddPostDialog(), PostTaskCompletion);
                    break;
                default:
                    await context.PostAsync("Sorry I didn't get you.");
                    context.Wait(MessageReceivedAsync);
                    break;
            }
        }

        private async Task PostTaskCompletion(IDialogContext context, IAwaitable<object> result)
        {
            context.Wait(MessageReceivedAsync);
        }
    }
}