using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using Autofac;
using CellarIQ.Bot.Dialogs;
using CellarIQ.Bot.Utilities;
using CellarIQ.Data;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Connector;

namespace CellarIQ.Bot
{
    public class CellarBotModule : Module
    {

        protected override void Load(ContainerBuilder builder)
        {

            string environment = ConfigurationManager.AppSettings["cellariq-environment"];

            base.Load(builder);

            CellarConfiguation cellarConfig = new CellarConfiguation();
            cellarConfig.CellarDatabaseConnectionUri = ConfigurationManager.AppSettings["cellariq-database-uri"];
            cellarConfig.CellarDatabaseKey = ConfigurationManager.AppSettings["cellariq-database-key"];
            cellarConfig.DatabaseId = ConfigurationManager.AppSettings["cellariq-database-id"];
            cellarConfig.SearchServiceName = ConfigurationManager.AppSettings["cellariq-searchservice-name"];
            cellarConfig.SearchServiceKey = ConfigurationManager.AppSettings["cellariq-searchservice-key"];

            builder.RegisterInstance(cellarConfig);

            builder.RegisterType<CellarManager>().UsingConstructor(typeof(CellarConfiguation));
            builder.RegisterType<CellarManagerResolver>().As<ICellarManagerResolver>();

            builder.Register(
                c => new LuisModelAttribute("ec69bd31-c4fb-4a63-bd05-e47a36148a69", "c1dfad2e93ff478a8f9dbcd4aaa95b71"))
                .AsSelf()
                .AsImplementedInterfaces()
                .InstancePerDependency();
            
           
            // register the top level dialog
            builder.RegisterType<CellarBotLuisDialog>()
                .As<IDialog<object>>()
                .UsingConstructor(typeof(ICellarManagerResolver) )
                .WithProperty("Environment", environment);

            //builder.RegisterType<RootDialog>().As<IDialog<object>>()
            //    .UsingConstructor(typeof(IMessageActivity))
            //    .WithProperty("Environment", environment);

            
            

        }
    }
}