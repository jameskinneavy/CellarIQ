using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using CellarIQ.Bot.Utilities;
using CellarIQ.Data;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis.Models;
using Action = Microsoft.Bot.Builder.Luis.Models.Action;

namespace CellarIQ.Bot.Dialogs
{
    public partial class CellarBotLuisDialog
    {
        [AuthenticateUserAspect]
        [LuisIntent("ShowCellarItems")]
        public async Task ShowCellarItems(IDialogContext context, LuisResult result)
        {

            var intent = result.Intents.SingleOrDefault(i => i.Intent.Equals("ShowCellarItems", StringComparison.OrdinalIgnoreCase));
            if (intent != null)
            {
                WineSearchParameters wineSearchParams = ExtractWineParametersFromIntentRecommendation(intent);

                var matchingItems = GetCellarItemsMatchingSpec(wineSearchParams);

                // Also, in case the label was really varietal, use that 
                if (wineSearchParams.WineLabel != null)
                {
                    CellarManager m = GetCellarManager();
                    var wines = m.GetAllWineLabels()
                        .Where(
                            w =>
                                w.Vintage == wineSearchParams.Vintage &&
                                w.VintnerName.ToLower().Contains(wineSearchParams.Vintner.ToLower()) &&
                                w.WineComposition.Count > 0 &&
                                w.WineComposition
                                    .Any(wc => wc.VarietalName.ToLower() == wineSearchParams.WineLabel.ToLower()));

                    foreach (var wine in wines)
                    {
                        if (!matchingItems.ContainsKey(wine))
                        {
                            matchingItems.Add(wine, m.GetAllCellarItems().Where(ci => ci.WineId.Equals(wine.Id, StringComparison.OrdinalIgnoreCase)).ToList());
                        }
                    }
                }


                StringBuilder responseBuilder = new StringBuilder();
                responseBuilder.AppendFormat("Here's what I found that matched your request:\n");
                foreach (var item in matchingItems)
                {
                    responseBuilder.AppendLine($"* {item.Key.Label} (x {item.Value.Count} bottle(s) )");
                }
                await context.PostAsync(responseBuilder.ToString());
                context.Wait(MessageReceived);
            }
            else
            {
                await context.PostAsync("Could not determine your intent");
                context.Wait(MessageReceived);
            }
        }

        

    }
}