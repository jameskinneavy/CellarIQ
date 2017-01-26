using System;
using System.Linq;
using System.Threading.Tasks;
using CellarIQ.Bot.Utilities;
using CellarIQ.Data;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis.Models;

namespace CellarIQ.Bot.Dialogs
{
    

    public partial class CellarBotLuisDialog
    {
        [AuthenticateUserAspect]
        [LuisIntent("GetWineLabelCount")]
        public async Task GetWineLabelCount(IDialogContext context, LuisResult result)
        {

            try
            {
                //var intent = result.Intents.Single(i => i.Intent.Equals("GetWineLabelCount", StringComparison.OrdinalIgnoreCase));
                WineSearchParameters wineSearchParams = ExtractWineParametersFromEntities(result.Entities);

                int count;

                var vintnerName = wineSearchParams.Vintner;
                if (vintnerName == null)
                {
                    count = GetCellarManager().GetAllWineLabels().Count();
                }
                else
                {
                    count = GetCellarManager().FindWineLabelsByVintner(vintnerName).Count();
                }

                string message;

                if (count > 0)
                {
                    message =
                        $"There are {count} different {(vintnerName != null ? vintnerName + " " : string.Empty)}wine labels in the cellar";

                }
                else
                {
                    message = $"I didn't find any wine labels by '{vintnerName}'";
                }

                await context.PostAsync(message);

            }
            catch (Exception ex)
            {
                await context.PostAsync($"An exception occured: {ex.Message}");
            }

            context.Wait(MessageReceived);
        }

    }


}