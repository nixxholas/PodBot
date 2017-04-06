using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace PodBotCSharp.Dialogs.User
{
    [Serializable]
    public class SetIGProfileDialog : IDialog<object>
    {
        private BotData _userActivity { get; set; }
        // Default Constructor for now
        public SetIGProfileDialog(BotData userActivity) {
            _userActivity = userActivity;
        }
    
        public async Task StartAsync(IDialogContext context)
        {
            
            context.Done(new object());
        }
    }
}