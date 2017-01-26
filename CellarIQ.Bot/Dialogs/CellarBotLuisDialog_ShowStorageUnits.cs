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
        [LuisIntent("ShowStorageUnits")]
        public async Task ShowStorageUnits(IDialogContext context, LuisResult result)
        {
            //TODO:Integrate with database
            List<StorageUnit> storageUnits = new List<StorageUnit>();
            storageUnits.Add(new StorageUnit {Capacity = 160, Name = "1", Description = "Vinotemp - Downstairs"});
            storageUnits.Add(new StorageUnit { Capacity = 104, Name = "2", Description = "Hall Closet - Unconditioned" });
            storageUnits.Add(new StorageUnit { Capacity = 104, Name = "3", Description = "Vinotemp - Upstairs" });

            string response = BuildShowStorageUnitsResponse(storageUnits);

            await context.PostAsync(response);
            context.Wait(MessageReceived);
        }

        private string BuildShowStorageUnitsResponse(List<StorageUnit> storageUnits)
        {
            StringBuilder responseBuilder = new StringBuilder();
            responseBuilder.AppendLine("Here are your storage units:");
            foreach (var u in storageUnits)
            {
                responseBuilder.AppendLine($"* Unit {u.Name}: {u.Description} with a capacity of {u.Capacity} bottles");
            }

            return responseBuilder.ToString();
        }
    }
}
    