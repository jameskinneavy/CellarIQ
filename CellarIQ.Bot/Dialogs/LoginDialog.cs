using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using CellarIQ.Bot.Utilities;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace CellarIQ.Bot.Dialogs
{
    public class LoginDialog : IDialog<object>
    {

        public string Environment { get; set; } = ConfigurationManager.AppSettings["Environment"];

        /// <summary>
        /// OAuth callback registered for Facebook app.
        /// <see cref="Controllers.OAuthCallbackController"/> implementats the callback.
        /// </summary>
        /// <remarks>
        /// Make sure to replace this with the appropriate website url registered for your Facebook app.
        /// </remarks>
        public static readonly Uri FacebookOauthCallback = new Uri(ConfigurationManager.AppSettings["cellariq-bot-uri"]
            + "/api/OAuthCallback");

        /// <summary>
        /// The key that is used to keep the AccessToken in <see cref="Microsoft.Bot.Builder.Dialogs.Internals.IBotData.PrivateConversationData"/>
        /// </summary>
        public static readonly string AuthTokenKey = "AuthToken";

        /// <summary>
        /// The pending message that is written to the <see cref="Microsoft.Bot.Builder.Dialogs.Internals.IBotData.PrivateConversationData"/>
        /// when bot is waiting for the response from the callback
        /// </summary>
        public readonly ResumptionCookie ResumptionCookie;



        /// <summary>
        /// Constructs an instance of the SimpleFacebookAuthDialog
        /// </summary>
        /// <param name="msg"></param>
        public LoginDialog(IMessageActivity msg)
        {
            ResumptionCookie = new ResumptionCookie(msg);

        }

        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }

        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            await LogIn(context);
        }

        /// <summary>
        /// Login the user.
        /// </summary>
        /// <param name="context"> The Dialog context.</param>
        /// <returns> A task that represents the login action.</returns>
        private async Task LogIn(IDialogContext context)
        {

            string token;
            if (!context.PrivateConversationData.TryGetValue(AuthTokenKey, out token))
            {
                context.PrivateConversationData.SetValue("persistedCookie", ResumptionCookie);

                // sending the sigin card with Facebook login url
                var reply = context.MakeMessage();
                var fbLoginUrl = FacebookHelpers.GetFacebookLoginURL(ResumptionCookie, FacebookOauthCallback.ToString());
                reply.Text = "Login Required";
                reply.Attachments.Add(SigninCard.Create("I'd love to help but first you will need to authorize me by logging into facebook",
                    "Login to Facebook!",
                    fbLoginUrl
                    ).ToAttachment());
                await context.PostAsync(reply);
                context.Wait(AfterLoginComplete);
            }
            else
            {
                context.Done(token);
            }
        }

        private async Task AfterLoginComplete(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var msg = await argument;
            if (msg.Text.StartsWith("token:"))
            {
                // Dialog is resumed by the OAuth callback and access token
                // is encoded in the message.Text
                var token = msg.Text.Remove(0, "token:".Length);

                var valid = await FacebookHelpers.ValidateAccessToken(token);
                var name = await FacebookHelpers.GetFacebookProfileName(token);
                context.UserData.SetValue("name", name);

                context.PrivateConversationData.SetValue(AuthTokenKey, token);

                var message = (await argument).Text;
                context.UserData.SetValue("message", message);
                //context.Call(new CellarBotLuisDialog(), AfterCellarBotSessionComplete);
                context.Done(message);
            }
            else
            {
                await LogIn(context);
            }
        }
    }
}