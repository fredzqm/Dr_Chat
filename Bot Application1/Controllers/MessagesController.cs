using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using Bot_Application1;
using Bot_Application1.Models;
using System.Collections.Generic;
using System.Diagnostics;

namespace Bot_Application1
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        /// 
        MedicInfo medic = new MedicInfo();

        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                // calculate something for us to return
                string answer = "Hmmmm, I didn't understand that.  I'm still learning!";
                string token = activity.Text.ToLower().Trim();
                Luis luisObj = await LUISClient.ParseUserInput(activity.Text);
                Debug.WriteLine(luisObj);
                if (luisObj.topScoringIntent != null)
                {
                    List<string> entityList = luisObj.getList();
                    String gender = "male";
                    int year = 1995;
                    switch (luisObj.topScoringIntent.intent) 
                    {
                        case "Diagnose":
                            answer = "You might be having a " + (medic.getDiagonoses(entityList, gender, year)).ToLower()+".";
                            break;
                        case "SeekingTreatments":
                            answer = medic.getTreatments(entityList);
                            break;
                        case "SeekiingSymptoms":
                            break;
                        case "Greetings":
                            answer = "Hello!";
                            break;
                        case "EndChat":
                            answer = "Bye!";
                            break;
                        case "None":
                            break;
                    }
                }
                Activity reply = activity.CreateReply(answer);
                await connector.Conversations.ReplyToActivityAsync(reply);
            }
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
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