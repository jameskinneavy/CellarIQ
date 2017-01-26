using System;
using System.Threading.Tasks;
using CellarIQ.Bot.Dialogs;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using PostSharp.Aspects;

namespace CellarIQ.Bot.Utilities
{
    [Serializable]
    public class AuthenticateUserAspect : OnMethodBoundaryAspect
    {
        private CellarBotLuisDialog _instance;

        public override void OnEntry(MethodExecutionArgs args)
        {
            _instance = args.Instance as CellarBotLuisDialog;
            if (_instance != null)
            {
                IDialogContext context = args.Arguments[0] as IDialogContext;
                if (context != null)
                {
                    string userName;
                    bool hasUserName = context.UserData.TryGetValue("username", out userName);
                    if (userName != null)
                    {
                        _instance.UserName = userName;
                    }
                    else
                    {
                        if (_instance.Environment != "Production")
                        {
                            //await context.PostAsync("Welcome, James. What can I do for you?");
                            //var message = (await argument).Text;
                            context.UserData.SetValue("name", "James");
                            //context.UserData.SetValue("message", message);
                        }
                        else
                        {
                            LoginDialog login = new LoginDialog(null);
                            //context.Call(login, _instance.);
                        }
                    }
                }
            }
        }

       
    }
}