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

namespace CellarIQ.Bot.Dialogs
{
    public partial class CellarBotLuisDialog
    {

        [AuthenticateUserAspect]
        [LuisIntent("ShowCellarItemsAtLocation")]
        public async Task ShowCellarItemsAtLocation(IDialogContext context, LuisResult result)
        {
            _wineSearchParameters = WineSearchParametersUtil.ExtractFromEntities(result.Entities);
            CellarManager cellarManager = GetCellarManager();
            var cellarItems =  cellarManager.FindCellarItems(_wineSearchParameters);

            string response = BuildShowItemsAtLocationResponse(cellarItems);

            await context.PostAsync(response);
            context.Wait(MessageReceived);
        }

        private string BuildShowItemsAtLocationResponse(IEnumerable<CellarItem> cellarItems)
        {
            cellarItems = cellarItems.ToList().OrderBy(ci => ci.StorageUnit).ThenBy(ci => ci.StorageShelf);

            StringBuilder responseBuilder = new StringBuilder();

            responseBuilder.AppendLine("Here's what I found:");

            string unit = "<default>";
            string shelf = "<default>";

            foreach (var item in cellarItems)
            {
                if (unit.ToLower() != item.StorageUnit.ToLower() || shelf.ToLower() != item.StorageShelf.ToLower())
                {
                    unit = item.StorageUnit;
                    shelf = item.StorageShelf;
                    responseBuilder.AppendFormat("* Storage {0}, Shelf {1}\n", unit, shelf);
                }
                responseBuilder.AppendFormat("   * {0} {1} {2}\n", item.WineInfo?.Vintage, item.WineInfo?.VintnerName, item.WineInfo?.Label);
            }

            return responseBuilder.ToString();
        }

    }
}