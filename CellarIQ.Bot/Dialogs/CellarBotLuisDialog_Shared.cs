using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using CellarIQ.Data;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis.Models;

namespace CellarIQ.Bot.Dialogs
{
    public partial class CellarBotLuisDialog
    {
        
        

        private Dictionary<Wine, List<CellarItem>> GetCellarItemsMatchingSpec(WineSearchParameters wineSearchParams)
        {
            CellarManager m = GetCellarManager();

            IEnumerable<Wine> wines = m.FindWines(wineSearchParams).ToList();

            List<CellarItem> cellarItems = m.GetAllCellarItems().ToList();

            // Add the cellar items for wines matching on partial vintage, vintner, and label
            Dictionary<Wine, List<CellarItem>> matchingItems = new Dictionary<Wine, List<CellarItem>>();
            foreach (var wine in wines)
            {
                List<CellarItem> items =
                    cellarItems.Where(ci => ci.WineId.Equals(wine.Id, StringComparison.OrdinalIgnoreCase)).ToList();

                items.ForEach(ci => ci.WineInfo = wine);

                matchingItems.Add(wine, items);
            }
            return matchingItems;
        }

        private static WineSearchParameters ExtractWineParametersFromEntities(IList<EntityRecommendation> entityRecommendations)
        {
            WineSearchParameters wineSearchParams = new WineSearchParameters();
            foreach (EntityRecommendation entityRecommendation in entityRecommendations)
            {
                wineSearchParams.SetParameter(entityRecommendation.Type, entityRecommendation.Entity);
            }

            return wineSearchParams;
        }

        private static WineSearchParameters ExtractWineParametersFromIntentRecommendation(IntentRecommendation intent)
        {
            WineSearchParameters wineSearchParams = new WineSearchParameters();
            Microsoft.Bot.Builder.Luis.Models.Action action = intent.Actions[0];
            foreach (ActionParameter actionParam in action.Parameters)
            {
                if (actionParam.Value != null)
                {
                    wineSearchParams.SetParameter(actionParam.Name, actionParam.Value[0].Entity);
                }
            }

            return wineSearchParams;
        }

        private static Dictionary<string, string> GetPossibleQuestions(bool useGeneralizedWineDescription)
        {
            Dictionary<string, string> possibleQuestions = new Dictionary<string, string>();
            possibleQuestions.Add("Vintage", "Vintage? Type ANY or ALL to ignore");
            possibleQuestions.Add("Vintner", "Vintner? Type ANY or ALL to ignore");
            possibleQuestions.Add(
                useGeneralizedWineDescription ? "WineDescription" : "WineLabel", "Label or varietal? Type ANY or ALL to ignore");

            return possibleQuestions;
        }

        private Dictionary<string, string> BuildQuestionsFromKnownEntities(IList<EntityRecommendation> entities)
        {
            bool useGeneralizedWineDescription = entities.SingleOrDefault(er => er.Type.Equals("WineDescription")) !=
                                                 null;

            var possibleQuestions = GetPossibleQuestions(useGeneralizedWineDescription);

            Dictionary<string, string> questions = new Dictionary<string, string>();
            foreach (var possibleQuestion in possibleQuestions)
            {

                string entityName = possibleQuestion.Key;
                EntityRecommendation entity =
                    entities.SingleOrDefault(e => e.Type.Equals(entityName, StringComparison.OrdinalIgnoreCase));

                var shouldAddQuestion = entity?.Entity == null;
                if (shouldAddQuestion)
                {
                    questions.Add(possibleQuestion.Key, possibleQuestion.Value);
                }
            }
            foreach (var entityRecommendation in entities)
            {
                if (entityRecommendation.Entity == null)
                {
                    questions.Add(entityRecommendation.Type, possibleQuestions[entityRecommendation.Type]);
                }
            }
            return questions;
        }
    }
}