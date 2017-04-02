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
            await context.PostAsync("activity.Text:" + activity.Text);

            await context.PostAsync("Sorry I didn't get you.");

            context.Wait(MessageReceivedAsync);
        }
    }
}