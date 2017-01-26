using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis.Models;

namespace CellarIQ.Bot.Dialogs
{
    public partial class CellarBotLuisDialog
    {
        [LuisIntent("OpenBottle")]
        public async Task OpenBottle(IDialogContext context, LuisResult result)
        {
            var wineParameters = ExtractWineParametersFromEntities(result.Entities);
            string description = $"";
        }
    }
}