using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;

namespace PodBotCSharp.Dialogs.User
{
    [Serializable]
    public class SetIGProfileDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            // Debugging Purposes
            //await context.PostAsync("Running SetProfileDialog");

            // Add the current user's information to the session first


            // Send the user to the Login API


            context.Done(new object());
        }
    }
}