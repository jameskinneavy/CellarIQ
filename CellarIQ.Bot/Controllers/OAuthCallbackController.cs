﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Autofac;
using CellarIQ.Bot.Dialogs;
using CellarIQ.Bot.Utilities;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;

namespace CellarIQ.Bot.Controllers
{
    public class OAuthCallbackController : ApiController
    {
        /// <summary>
        /// OAuth call back that is called by Facebook. Read https://developers.facebook.com/docs/facebook-login/manually-build-a-login-flow for more details.
        /// </summary>
        /// <param name="userId"> The Id for the user that is getting authenticated.</param>
        /// <param name="conversationId"> The Id of the conversation.</param>
        /// <param name="code"> The Authentication code returned by Facebook.</param>
        /// <param name="state"> The state returned by Facebook.</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/OAuthCallback")]
        public async Task<HttpResponseMessage> OAuthCallback([FromUri] string userId, [FromUri] string botId, [FromUri] string conversationId, [FromUri] string channelId, [FromUri] string serviceUrl, [FromUri] string locale, [FromUri] string code, [FromUri] string state, CancellationToken token)
        {
            // Get the resumption cookie
            var address = new Address
                (
                    // purposefully using named arguments because these all have the same type
                    botId: FacebookHelpers.TokenDecoder(botId),
                    channelId: channelId,
                    userId: FacebookHelpers.TokenDecoder(userId),
                    conversationId: FacebookHelpers.TokenDecoder(conversationId),
                    serviceUrl: FacebookHelpers.TokenDecoder(serviceUrl)
                );
            var resumptionCookie = new ResumptionCookie(address, userName: null, isGroup: false, locale: locale);

            // Exchange the Facebook Auth code with Access token
            var accessToken = await FacebookHelpers.ExchangeCodeForAccessToken(
                resumptionCookie, code, RootDialog.FacebookOauthCallback.ToString());

            // Create the message that is send to conversation to resume the login flow
            var msg = resumptionCookie.GetMessage();
            msg.Text = $"token:{accessToken.AccessToken}";

            // Resume the conversation to SimpleFacebookAuthDialog
            await Conversation.ResumeAsync(resumptionCookie, msg);

            using (var scope = DialogModule.BeginLifetimeScope(Conversation.Container, msg))
            {
                var dataBag = scope.Resolve<IBotData>();
                await dataBag.LoadAsync(token);
                ResumptionCookie pending;
                if (dataBag.PrivateConversationData.TryGetValue("persistedCookie", out pending))
                {
                    // remove persisted cookie
                    dataBag.PrivateConversationData.RemoveValue("persistedCookie");
                    await dataBag.FlushAsync(token);

                    HttpResponseMessage response = new HttpResponseMessage
                    {
                        Content =
                            new StringContent("<html><body>You are now logged in! Continue talking to the bot.</html>",Encoding.UTF8, "text/html")
                        
                    };
                    //response.Headers.Add("Content-Type", "text/html");
                    return Request.CreateResponse("You are now logged in! Continue talking to the bot.");
                }
                else
                {
                    // Callback is called with no pending message as a result the login flow cannot be resumed.
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, new InvalidOperationException("Cannot resume!"));
                }
            }
        }
    }
}