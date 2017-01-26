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
        [LuisIntent("FindCellarItems")]
        public async Task FindCellarItems(IDialogContext context, LuisResult result)
        {
            
            _questionNo = 0;
            _questions = new Dictionary<string, string>();
            _answers = new Dictionary<string, string>();
            _questions = BuildQuestionsFromKnownEntities(result.Entities);
            _wineSearchParameters = ExtractWineParametersFromEntities(result.Entities);
     
            // Let's only ask if they didn't provide at least
            if (_questions.Count > 1)
            {
                string question = _questions.Values.ElementAt(0);
                PromptDialog.PromptString dialog = new PromptDialog.PromptString(question, question, 3);
                context.Call(dialog, HandleFindCellarItemsDialogAsync);
            }
            else
            {
                var results = GetCellarManager().FindCellarItems(_wineSearchParameters);
                string response = BuildFindCellarItemResults(DataUtility.GroupCellarItemsByWine(results));

                await context.PostAsync(response);
                context.Wait(MessageReceived);
            }

            
        }

        private async Task HandleFindCellarItemsDialogAsync(IDialogContext context, IAwaitable<string> result)
        {
            var answer = await result;
            string key = _questions.Keys.ElementAt(_questionNo);
            _answers.Add(key, answer.ToLower());

            _questionNo++;


            if (_questionNo < _questions.Count)
            {
                string nextQuestion = _questions.Values.ElementAt(_questionNo);
                PromptDialog.PromptString dialog = new PromptDialog.PromptString(nextQuestion, nextQuestion, 3);
                context.Call(dialog, HandleFindCellarItemsDialogAsync);
            }
            else
            {
                _wineSearchParameters.SetParameters(_answers);
                var results = GetCellarManager().FindCellarItems(_wineSearchParameters);
                string response = BuildFindCellarItemResults(DataUtility.GroupCellarItemsByWine(results));

                await context.PostAsync(response);
                context.Wait(MessageReceived);

            }
        }

        private static string BuildFindCellarItemResults(Dictionary<Wine, IEnumerable<CellarItem>> cellarItemMap)
        {
            string response;


            if (cellarItemMap.Any())
            {
                StringBuilder responseBuilder = new StringBuilder();
                responseBuilder.AppendFormat("Good news! I think I found what you were looking for:\n");

                foreach (var item in cellarItemMap.Keys)
                {
                    Wine wine = item;
                    responseBuilder.AppendLine($"* {wine.VintnerName} {(wine.Vintage != "NONE" ? wine.Vintage + " " : string.Empty)}{wine.Label}\n");
                    var cellarItems =
                        cellarItemMap[item].GroupBy(ci => "Unit " + ci.StorageUnit + " - Shelf " + ci.StorageShelf);
                    foreach (var cellarItemGroup in cellarItems)
                    {
                        responseBuilder.AppendLine(" * " + cellarItemGroup.Key + " (x" + cellarItemGroup.Count() + ")");

                    }
                }

                response = responseBuilder.ToString();
            }
            else
            {
                response = "Bummer! I didn't find what you were looking for";
            }

            return response;
        }
    }
}