using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Autofac;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;

namespace CellarIQ.Bot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {

        // TODO: "service locator"
        private readonly ILifetimeScope scope;
        public MessagesController(ILifetimeScope scope)
        {
            SetField.NotNull(out this.scope, nameof(scope), scope);
        }

        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity, CancellationToken token)
        {

            if (activity != null)
            {
                switch (activity.GetActivityType())
                {
                    case ActivityTypes.Message:
                        using (var scope = DialogModule.BeginLifetimeScope(this.scope, activity))
                        {
                            var postToBot = scope.Resolve<IPostToBot>();
                            await postToBot.PostAsync(activity, token);
                        }

                        break;
                    case ActivityTypes.ConversationUpdate:

                        ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                        await connector.Conversations.ReplyToActivityAsync(
                            activity.CreateReply("Hello! I'm the CellarBot. What can I do for you?"));
                        break;
                }
            }

            return new HttpResponseMessage(HttpStatusCode.Accepted);
            //if (activity.Type == ActivityTypes.Message)
            //{

            //    await Conversation.SendAsync(activity, () => new CellarBotLuisDialog());

            //    //ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
            //    //// calculate something for us to return
            //    //int length = (activity.Text ?? string.Empty).Length;

            //    //// return our reply to the user
            //    //Activity reply = activity.CreateReply($"You sent {activity.Text} which was {length} characters");
            //    //await connector.Conversations.ReplyToActivityAsync(reply);
            //}
            //else
            //{
            //    HandleSystemMessage(activity);
            //}
            //var response = Request.CreateResponse(HttpStatusCode.OK);
            //return response;
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}