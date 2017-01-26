using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Autofac;
using CellarIQ.Data;

namespace CellarIQ.Bot.Utilities
{
    [Serializable]
    public class CellarManagerResolver : ICellarManagerResolver
    {
        public CellarManager Get()
        {
            return WebApiApplication.FindContainer().Resolve<CellarManager>();
        }
    }

    
    public interface ICellarManagerResolver
    {
        CellarManager Get();
    }
}