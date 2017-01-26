using System;
using CellarIQ.Bot.Dialogs;
using Microsoft.Bot.Builder.Dialogs;
using PostSharp.Aspects;

namespace CellarIQ.Bot.Utilities
{
    [Serializable]
    public class SetUserNameAspect : OnMethodBoundaryAspect
    {

        public override void OnEntry(MethodExecutionArgs args)
        {
          
            CellarBotLuisDialog instance = args.Instance as CellarBotLuisDialog;
            if (instance != null)
            {
                IDialogContext context = args.Arguments[0] as IDialogContext;
                if (context != null)
                {
                    instance.UserName = context.UserData.Get<string>("name");
                }
            }
            
        }
    }
}