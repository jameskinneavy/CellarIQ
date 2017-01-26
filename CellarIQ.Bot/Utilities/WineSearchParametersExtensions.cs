using System.Collections.Generic;
using CellarIQ.Data;
using Microsoft.Bot.Builder.Luis.Models;

namespace CellarIQ.Bot.Utilities
{
    public static class WineSearchParametersUtil
    {
        public static WineSearchParameters ExtractFromEntities(IList<EntityRecommendation> entityRecommendations)
        {

            WineSearchParameters wineSearchParams = new WineSearchParameters();
            foreach (EntityRecommendation entityRecommendation in entityRecommendations)
            {
                wineSearchParams.SetParameter(entityRecommendation.Type, entityRecommendation.Entity);
            }

            return wineSearchParams;
        }
        
        
    }
   
}