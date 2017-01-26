using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
        [LuisIntent("ShowWinesBySpecification")]
        public async Task ShowWinesBySpecification(IDialogContext context, LuisResult result)
        {
            string query = result.Query;

            _questionNo = 0;
            _questions = new Dictionary<string, string>();
            _answers = new Dictionary<string, string>();
            //_questions = BuildQuestionsFromKnownEntities(result.Entities);
            _wineSearchParameters = WineSearchParametersUtil.ExtractFromEntities(result.Entities);
            


            string notFoundResponse = null;
            IEnumerable<Wine> results = null;
            if (query.Contains("blend"))
            {
                //Expression<Func<Wine, bool>> wineSearchPredicate = wine => w;
                results = GetCellarManager().GetAllWineLabels().Where(w => w.WineComposition.Count > 0);
                notFoundResponse = "Sorry, there aren't any blended wines in your cellar";
            }

            if (query.Contains("contain"))
            {
                if (_wineSearchParameters.Varietal != null)
                {
                    string varietal = _wineSearchParameters.Varietal.ToLower();
                    //Expression<Func<string, WineComposition>> p = varietal => 
                    Expression<Func<Wine, bool>> wineSearchPredicate =
                        wine => wine.Label.ToLower().Contains(varietal) ||
                                wine.WineComposition.Any(
                                    wc => wc.VarietalName.ToLower().Contains(varietal));

                    results = GetCellarManager().GetAllWineLabels()
                        .Where(
                            wine => wine.Label.ToLower().Contains(varietal) ||
                                    wine.WineComposition.Any(wc => wc.VarietalName.ToLower().Contains(varietal)));

                    notFoundResponse = "Sorry, there are no wines in your cellar containing " + varietal;
                }

            }

            string response = results != null
                ? BuildGetWineDetailResults(results)
                : notFoundResponse ?? "Sorry, I'm not sure what you were asking?";

            await context.PostAsync(response);
            context.Wait(MessageReceived);
        }

        private async Task HandleShowWinesBySpecificationDialogAsync(IDialogContext context, IAwaitable<string> result)
        {

            var answer = await result;
            string key = _questions.Keys.ElementAt(_questionNo);
            _answers.Add(key, answer.ToLower());

            _questionNo++;
            // use Count instead of Length in case it is not an array
            if (_questionNo < _questions.Count)
            {
                string nextQuestion = _questions.Values.ElementAt(_questionNo);
                PromptDialog.PromptString dialog = new PromptDialog.PromptString(nextQuestion, nextQuestion, 3);
                context.Call(dialog, HandleDescribeWineDialogAsync);
            }
            else
            {
                
            }

            await context.PostAsync("This method not implemented");
            context.Wait(MessageReceived);
        }
    }
}