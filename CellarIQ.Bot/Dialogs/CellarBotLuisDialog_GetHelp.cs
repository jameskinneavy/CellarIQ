using System;
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
        [LuisIntent("GetHelp")]
        public async Task GetHelp(IDialogContext context, LuisResult result)
        {

            try
            {
                StringBuilder responseBuilder = new StringBuilder();
                responseBuilder.AppendLine(
                    "Hey there! I'm your friendly CellarBot and I'm eager to help you with your wine cellar. There are many interesting questions or commands you can pose such as:");
                responseBuilder.AppendLine("* How many wine labels are in the cellar?");
                responseBuilder.AppendLine("* How many bottles of wine?");
                responseBuilder.AppendLine("* Counts and lists for vintages, vintners and labels...e.g. Try 'How many bottles of Beckmen 2009 PMV Syrah?'");
                responseBuilder.AppendLine("* Tell me about <vintner> <vintage> <label> (you can enter some or all of the data)");
                responseBuilder.AppendLine("* Tell me about a wine. (I will ask you questions)");
                responseBuilder.AppendLine("* Find me a bottle of wine. (I will ask you questions)");
                responseBuilder.AppendLine("* What is in unit <name-or-number> on shelf <number>?  (for example)");
                responseBuilder.AppendLine("* Show vintners");
                responseBuilder.AppendLine("* Show blends");
                responseBuilder.AppendLine("* Show wines containing <varietal>");
                responseBuilder.AppendLine("* Show storage units");





                await context.PostAsync(responseBuilder.ToString());

            }
            catch (Exception ex)
            {
                await context.PostAsync($"An exception occured: {ex.Message}");
            }

            context.Wait(MessageReceived);
        }
    }
}