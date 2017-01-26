using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace CellarIQ.Bot.Dialogs
{
    [Serializable]
    public class TestDialog : IDialog<string>
    {
        public async Task StartAsync(IDialogContext context)
        {
            //string message;
            //bool messageExists = context.UserData.TryGetValue("message", out message);
            //if (messageExists)
            //{
            //    context.UserData.RemoveValue("message");
            //    await context.PostAsync($"You said {message}");
            //}
            context.Wait(MessageReceivedAsync);
            //await MessageReceivedAsync()
        }

        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;
            
            await context.PostAsync($"You said {message.Text}");
            context.Wait(MessageReceivedAsync);
            
        }
    }
}