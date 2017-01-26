using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        [LuisIntent("GetCellarItemCount")]
        public async Task GetCellarItemCount(IDialogContext context, LuisResult result)
        {

            try
            {

                WineSearchParameters wineSearchParams = ExtractWineParametersFromEntities(result.Entities);

                var wines = GetCellarItemsMatchingSpec(wineSearchParams).Select(i => i.Value).SelectMany(list => list).Distinct().ToList();
                int count = wines.Count();
                string details = $"{wineSearchParams.Vintage} {wineSearchParams.Vintner} {wineSearchParams.WineLabel}{wineSearchParams.WineDescription}".Trim();
                var response = $"I'm sorry, {UserName}, but I could not find any bottles of {details}";

                if (count > 0)
                {
                    response = $"{UserName}, There are {count} bottles of {details} in the cellar. Would you like to see them?";
                    context.PrivateConversationData.SetValue("Items", wines);
                    PromptDialog.Confirm(context, HandleResultOfViewItemsCountAsync, response);
                }
                else
                {
                    await context.PostAsync(response);
                    context.Wait(MessageReceived);
                }
                
                
                //

            }
            catch (Exception ex)
            {
                await context.PostAsync($"Something went wrong! {ex.Message}");
                context.Wait(MessageReceived);
            }

            
        }

        private async Task HandleResultOfViewItemsCountAsync(IDialogContext context, IAwaitable<bool> result)
        {
            bool showResults = result.GetAwaiter().GetResult();

            if (showResults)
            {
                var cellarItems = context.PrivateConversationData.Get<IEnumerable<CellarItem>>("Items");
                context.PrivateConversationData.RemoveValue("Items");
                string response = BuildShowCellarItems(cellarItems);
                await context.PostAsync(response);
                
            }

            context.Wait(MessageReceived);
        }

        private string BuildShowCellarItems(IEnumerable<CellarItem> cellarItems)
        {

            Dictionary<Wine, IEnumerable<CellarItem>> cellarItemMap = DataUtility.GroupCellarItemsByWine(cellarItems);
            
            StringBuilder responseBuilder = new StringBuilder();
            responseBuilder.AppendFormat("Okay, here's the list of those items:\n");

            foreach (var item in cellarItemMap.Keys)
            {
                Wine wine = item;

                int count = cellarItemMap[wine].Count();
                responseBuilder.AppendLine($"* {wine.VintnerName} {(wine.Vintage != "NONE" ? wine.Vintage + " " : string.Empty)}{wine.Label} (x {count})");
                
            }

            string response = responseBuilder.ToString();

            return response;
        }
    }
}