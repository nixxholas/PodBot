using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace HushienBot.Dialogs
{
    [LuisModel("e01e77ec-d2a9-47a1-bf48-5c37ef5055ad", "3d9fbecb4439471ca22498c171ed9716")]
    [Serializable]
    public class BasicDialog : LuisDialog<object>
    {
        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            context.Wait(MessageReceived);
        }

        [LuisIntent("myBirthday")]
        public async Task myBirthday(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("My birthday is on the February 19, 1971");
            context.Wait(MessageReceived);
        }

        [LuisIntent("Birthday")]
        public async Task Birthday(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Oh happy birthday. Let the new age begin and end with new principles, habits and gogiver values.");
            context.Wait(MessageReceived);
        }

        [LuisIntent("pervert")]
        public async Task Pervert(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Don't be disgusting hor");
            context.Wait(MessageReceived);
        }

        [LuisIntent("Initiator")]
        public async Task Greeting(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("hami?");
            //await context.PostAsync("Yes?");
            context.Wait(MessageReceived);
        }

        [LuisIntent("EmpathyInfo")]
        public async Task getEmpathyInfo(IDialogContext context, LuisResult result)
        {
            try
            {
                // *************************
                // Log to Database
                // *************************
                // Instantiate the BotData dbContext
                Models.hushienDBEntities DB = new Models.hushienDBEntities();

                // Retrieve the DB object
                // http://stackoverflow.com/questions/654906/linq-to-entities-random-order
                List<Models.Empathy> empathy_ = DB.Empathies
                    .ToList();

                foreach (Models.Empathy currEmpathy in empathy_)
                {
                    if (!currEmpathy.description.Equals("") || !currEmpathy.description.Equals(null))
                    {
                        await context.PostAsync(currEmpathy.description);
                    }

                    if (!currEmpathy.url.Equals("") || !currEmpathy.url.Equals(null))
                    {
                        await context.PostAsync(currEmpathy.url);
                    }
                }
            } catch (Exception e) {
                await context.PostAsync(e.Message);
            }
        }

        [LuisIntent("Preaching")]
        public async Task Preach(IDialogContext context, LuisResult result)
        {
            // *************************
            // Log to Database
            // *************************
            // Instantiate the BotData dbContext
            Models.hushienDBEntities DB = new Models.hushienDBEntities();

            // Instantiate the Randomizer Object
            // http://stackoverflow.com/questions/2706500/how-do-i-generate-a-random-int-number-in-c
            Random rnd = new Random();

            if (result.Entities.Count != 0) // Check if the entity is properly parsed in or not
            {
                try
                {
                    // Convert the result to string
                    string resultant = result.Entities[0].Entity.ToString();

                    // Create a new UserLog object
                    //Models.UserLog NewUserLog = new Models.UserLog();
                    // Set the properties on the UserLog object
                    //NewUserLog.Channel = activity.ChannelId;
                    //NewUserLog.UserID = activity.From.Id;
                    //NewUserLog.UserName = activity.From.Name;
                    //NewUserLog.created = DateTime.UtcNow;
                    //NewUserLog.Message = activity.Text.Truncate(500);
                    // Add the UserLog object to UserLogs
                    //DB.UserLogs.Add(NewUserLog);


                    // Retrieve the DB object
                    // http://stackoverflow.com/questions/654906/linq-to-entities-random-order
                    List<Models.Preach> preach_ = DB.Preaches
                        .Where(input => input.preachType.Equals(resultant))
                        .ToList();

                    await context.PostAsync(preach_[rnd.Next(0, preach_.Count() - 1)].preachSentence);

                } catch (Exception e)
                {
                    //await context.PostAsync(e.ToString());   

                }

                // Save the changes to the database
                //DB.SaveChanges();
                // Call NumberGuesserDialog
                //await Conversation.SendAsync(activity, () => new NumberGuesserDialog());
            } else {

                List<Models.Preach> preach_ = DB.Preaches
                    .Where(input => input.preachType.Equals("none"))
                    .ToList();

                await context.PostAsync(preach_[rnd.Next(0, preach_.Count() - 1)].preachSentence);
            }

            //if (result.Entities.Count != 0) { }
            //EntityRecommendation e = result.Entities[0]; // Get the entity that's recommended by LUIS
            //string stringResult = result.Entities[0].Entity;

            context.Wait(MessageReceived);
        }

        [LuisIntent("Timetable")]
        public async Task Timetable(IDialogContext context, LuisResult result)
        {
            // We'll have to perform checks for the specific day
            //Generate a message
            // https://docs.botframework.com/en-us/csharp/builder/sdkreference/attachments.html
            var message = context.MakeMessage();
            message.Attachments = new List<Attachment>();
            
            //Generate buttons
            List<CardAction> buttonList = new List<CardAction>();
            buttonList.Add(new CardAction()
            {
                Value = "1",
                Type = "postBack",
                Title = "Monday"
            });

            buttonList.Add(new CardAction()
            {
                Value = "2",
                Type = "postBack",
                Title = "Tuesday"
            });

            buttonList.Add(new CardAction()
            {
                Value = "3",
                Type = "postBack",
                Title = "Wednesday"
            });

            buttonList.Add(new CardAction()
            {
                Value = "4",
                Type = "postBack",
                Title = "Thursday"
            });

            buttonList.Add(new CardAction()
            {
                Value = "5",
                Type = "postBack",
                Title = "Friday"
            });

            //Generate card
            var card = new ThumbnailCard()
            {
                Title = "Which Day?",
                Text = "I'll check it out for you",
                //Images = imageList,
                Buttons = buttonList
            };

            //Add card to message
            message.Attachments.Add(card.ToAttachment());

            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }

        [LuisIntent("Chairman")]
        public async Task Chairman(IDialogContext context, LuisResult result)
        {
            //Generate a message
            var message = context.MakeMessage();
            message.Attachments = new List<Attachment>();

            //Generate images
            List<CardImage> imageList = new List<CardImage>();
            imageList.Add(new CardImage(url: "https://www.tech.gov.sg/-/media/GovTech/About-us/Board-Of-Directors/Mr-Ng-Chee-Khern.jpg"));

            //Generate buttons
            List<CardAction> buttonList = new List<CardAction>();
            buttonList.Add(new CardAction()
            {
                Value = "https://www.tech.gov.sg/en/About-Us/Organisation-Team/Board-of-Directors",
                Type = "openUrl",
                Title = "See all directors"
            });

            //Generate card
            var card = new HeroCard()
            {
                Title = "Mr Ng Chee Khern",
                Text = "Chairman of GovTech",
                Images = imageList,
                Buttons = buttonList
            };

            //Add card to message
            message.Attachments.Add(card.ToAttachment());

            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }

        //[LuisIntent("News")]
        //public async Task News(IDialogContext context, LuisResult result)
        //{
        //    //Make card carousel
        //    var message = context.MakeMessage();
        //    message.AttachmentLayout = AttachmentLayoutTypes.Carousel;
        //    message.Attachments = new List<Attachment>();

        //    //Retrieve news from the web service
        //    List<NewsArticle> articleList = await NewsArticleAccess.GetNewsArticlesAsync();

        //    //Make cards, limit to 5 cause carousel only supports 5
        //    for (int i = 0; i < 5 && i < articleList.Count; i++)
        //    {
        //        //Assign current iteration to variable
        //        var article = articleList[i];

        //        //Generate buttons
        //        List<CardAction> buttonList = new List<CardAction>();
        //        buttonList.Add(new CardAction
        //        {
        //            Value = $"{article.ArticleURL}",
        //            Type = "openUrl",
        //            Title = "More details"
        //        });

        //        //Generate images
        //        List<CardImage> imageList = new List<CardImage>();
        //        imageList.Add(new CardImage(article.ImageURL));

        //        //Generate card
        //        HeroCard card = new HeroCard()
        //        {
        //            Title = article.Title,
        //            Subtitle = article.Date,
        //            Images = imageList,
        //            Buttons = buttonList
        //        };

        //        //Add card to message
        //        message.Attachments.Add(card.ToAttachment());
        //    }

        //    //Send messages
        //    await context.PostAsync("Here are the top news:");
        //    await context.PostAsync(message);
        //    await context.PostAsync("You can see more news at https://www.tech.gov.sg/en/TechNews.");

        //    context.Wait(MessageReceived);
        //}

        [LuisIntent("ppap")]
        public async Task PPAP(IDialogContext context, LuisResult result)
        {
            // http://stackoverflow.com/questions/26798845/await-task-delay-vs-task-delay-wait
            await context.PostAsync("P-P-A-P");
            //await Task.Delay(500);
            //await context.PostAsync("I have a pen");
            //await context.PostAsync("I have a apple");
            //await Task.Delay(500);
            //await context.PostAsync("uh");
            //await Task.Delay(500);
            //await context.PostAsync("Apple Pen");
            //await Task.Delay(500);
            //await context.PostAsync("I have a pen");
            //await context.PostAsync("I have pineapple");
            //await Task.Delay(1000);
            //await context.PostAsync("uh");
            //await context.PostAsync("Pineapple Pen");
            //await Task.Delay(500);
            //await context.PostAsync("Apple Pen, Pineapple Pen");
            //await Task.Delay(500);
            //await context.PostAsync("Uh");
            //await Task.Delay(500);
            //await context.PostAsync("Pen Pineapple, Apple Pen");
            context.Wait(MessageReceived);
        }
}
}