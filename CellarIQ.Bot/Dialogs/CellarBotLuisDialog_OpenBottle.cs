using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using CellarIQ.Bot.Utilities;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis.Models;

namespace CellarIQ.Bot.Dialogs
{
    public partial class CellarBotLuisDialog
    {
        [LuisIntent("OpenBottle")]
        public async Task OpenBottle(IDialogContext context, LuisResult result)
        {
            var wineParameters = WineSearchParametersUtil.ExtractFromEntities(result.Entities);
            string reply = "While I'd love to open a bottle of " +
                                 $"{wineParameters.Vintner} {wineParameters.Vintage} {wineParameters.Varietal}, " +
                                 "but I'm just a ChatBot. I don't have any hands.";

            context.PostAsync(reply);

            context.Wait(MessageReceived);
        }
    }
}