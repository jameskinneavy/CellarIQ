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
        [LuisIntent("DescribeWine")]
        public async Task DescribeWine(IDialogContext context, LuisResult result)
        {
            _questionNo = 0;
            _questions = new Dictionary<string, string>();
            _answers = new Dictionary<string, string>();

            _wineSearchParameters = ExtractWineParametersFromEntities(result.Entities);
            _questions = BuildQuestionsFromKnownEntities(result.Entities);
            if (_questions.Count > 0)
            {
                string question = _questions.Values.ElementAt(0);
                PromptDialog.PromptString dialog = new PromptDialog.PromptString(question, question, 3);
                context.Call(dialog, HandleDescribeWineDialogAsync);
            }
            else
            {
                var wines = GetCellarManager().FindWines(_wineSearchParameters);
                var queryResults = BuildGetWineDetailResults(wines);

                await context.PostAsync(queryResults);
                context.Wait(MessageReceived);
            }
            
        }
        

        private async Task HandleDescribeWineDialogAsync(IDialogContext context, IAwaitable<string> result)
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
                _wineSearchParameters.SetParameters(_answers);
                var wines = GetCellarManager().FindWines(_wineSearchParameters);
                var queryResults = BuildGetWineDetailResults(wines);

                await context.PostAsync(queryResults);
                context.Wait(MessageReceived);

            }
        }

        private static string BuildGetWineDetailResults(IEnumerable<Wine> wines)
        {
            string queryResults;
            var wineList
                = wines as IList<Wine> ?? wines.ToList();
            if (wineList.Any())
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("I found {0} wines matching your inquiry:\n", wineList.Count());


                foreach (var wine in wineList)
                {
                    sb.Append($"* {(wine.Vintage == "NONE" ? string.Empty : wine.Vintage) } {wine.VintnerName} {wine.Label} ");
                    if (wine.WineComposition.Count == 1)
                    {
                        sb.Append(
                            $"is {wine.WineComposition[0].PercentComposition}% {wine.WineComposition[0].VarietalName}. ");
                    }
                    else if (wine.WineComposition.Count > 1)
                    {
                        sb.Append("is a blend of ");
                        foreach (var comp in wine.WineComposition)
                        {
                            sb.AppendFormat("{0}% {1}, ", comp.PercentComposition, comp.VarietalName);
                        }
                        sb.Length = sb.Length - 2;
                    }

                    sb.AppendFormat(". It is {0}% ABV.", wine.PercentAlcohol);
                    sb.AppendLine();
                }
                queryResults = sb.ToString();
            }
            else
            {
                queryResults = "No matching wines were found.";
            }
            return queryResults;
        }


       
    }
}