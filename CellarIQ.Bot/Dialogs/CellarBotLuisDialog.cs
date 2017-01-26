using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using CellarIQ.Bot.Utilities;
using CellarIQ.Data;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using PostSharp.Aspects;
using Action = Microsoft.Bot.Builder.Luis.Models.Action;

namespace CellarIQ.Bot.Dialogs
{

    

    
    [LuisModel("90d7e8f6-170d-4771-a123-eedf1734c6e0", "c1dfad2e93ff478a8f9dbcd4aaa95b71")]
    [Serializable]
    public partial class CellarBotLuisDialog : LuisDialog<object>
    {
        public string Environment { get; set; }

        public ICellarManagerResolver CellarManagerResolver { get; set; }


        private Dictionary<string, string> _questions;
        private Dictionary<string, string> _answers;
        private int _questionNo;
        private WineSearchParameters _wineSearchParameters;
        public string UserName { get; set; }


        public CellarBotLuisDialog(ICellarManagerResolver cellarManagerResolver)
        {
            CellarManagerResolver = cellarManagerResolver;
        }



        private CellarManager GetCellarManager()
        {
            return CellarManagerResolver.Get();
        }


        [AuthenticateUserAspect]
        [LuisIntent("")]
        public async Task NoIntent(IDialogContext context, LuisResult result)
        {
            string response = "I didn't understand your request. Please try again";

            string query = result.Query.ToLower();

            if (query.Contains("skynet"))
            {
                response = "No, I'm not SkyNet, but do you think I'd tell you if I were?!?";
            }

            if (query.Contains("murder") || query.Contains("kill"))
            {
                response = "I'm really not violent unless you try to drink all of my wine";
            }

            await context.PostAsync(response);
            context.Wait(MessageReceived);
        }

    }
}