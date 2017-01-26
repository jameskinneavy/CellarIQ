using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CellarIQ.Bot.Utilities;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis.Models;

namespace CellarIQ.Bot.Dialogs
{
    public partial class CellarBotLuisDialog
    {
        [AuthenticateUserAspect]
        [LuisIntent("ShowVintners")]
        public async Task ShowVintners(IDialogContext context, LuisResult result)
        {
            var vintners = GetCellarManager().GetAllWineLabels().Select(w => w.VintnerName).Distinct().OrderBy(v => v);
            StringBuilder responseBuilder = new StringBuilder();
            responseBuilder.AppendFormat("Here's the list of vintners in your cellar:\n");
            foreach (var item in vintners)
            {
                responseBuilder.AppendLine($"* {item}");
            }
            await context.PostAsync(responseBuilder.ToString());
            context.Wait(MessageReceived);
        }

    }
}