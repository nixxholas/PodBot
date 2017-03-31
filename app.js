require('dotenv').config()

const http = require('http');
const express = require('express');
const restify = require('restify');
const builder = require('botbuilder');
const Client = require('instagram-private-api').V1;

//=========================================================
// External API Setup
//=========================================================

//=========================================================
// Bot Setup
//=========================================================

// Setup the main process
const app = express();
app.set('port', 3979);

// Setup Restify Server
const server = restify.createServer();
server.listen(process.env.port || process.env.PORT || 3978, function () {
   console.log('%s listening to %s', server.name, server.url); 
});
  
// Create chat bot
const connector = new builder.ChatConnector({
    appId: process.env.MICROSOFT_APP_ID,
    appPassword: process.env.MICROSOFT_APP_PASSWORD
});
const bot = new builder.UniversalBot(connector);
server.post('/api/messages', connector.listen());

//=========================================================
// Instagram OAuth Setup
//=========================================================

//=========================================================
// Bots Dialogs
//=========================================================

const intents = new builder.IntentDialog();
bot.dialog('/', intents);

intents.matches(/^setupprofile/i, '/setupprofile')
        .matches();

intents.onDefault(
    // On default, we'll do nothing.
    function (session, args, next) {
    }
);

bot.dialog([
    // Function to help instantiate the user's session with PodBot.
    '/setupprofile', [
        function (session) {
            // Expect the bot to receive a reply from the user, thus use Prompts
            builder.Prompts.text(session, 'Hi! What is your handle?');
        },
        function (session, results) {
            // Retrieve the user's response
            session.userData.name = results.response;

            // Handle oAuth first

            // Since we're done, display the user details as a card to let
            // him know that we're done
            var card = createThumbnailCard(session, session.userData.name);

            // attach the card to the reply message
            var msg = new builder.Message(session).addAttachment(card);
            session.send(msg);
            
            //session.endDialog();
        }
    ],
    '/likeall', [
        function (session) {
            // Expect the bot to receive a reply from the user, thus use Prompts
            builder.Prompts.text(session, 'Hi! What is your handle?');
        },
        function (session, results) {
            // Retrieve the user's response
            session.userData.name = results.response;

            // Handle oAuth first

            // Since we're done, display the user details as a card to let
            // him know that we're done
            var card = createThumbnailCard(session, session.userData.name);

            // attach the card to the reply message
            var msg = new builder.Message(session).addAttachment(card);
            session.send(msg);
            
            //session.endDialog();
        }
    ]
]);

function createThumbnailCard(session, handle) {
    return new builder.ThumbnailCard(session)
        .title('BotFramework Thumbnail Card')
        .subtitle('Your bots — wherever your users are talking')
        .text('Build and connect intelligent bots to interact with your users naturally wherever they are, from text/sms to Skype, Slack, Office 365 mail and other popular services.')
        .images([
            builder.CardImage.create(session, 'https://sec.ch9.ms/ch9/7ff5/e07cfef0-aa3b-40bb-9baa-7c9ef8ff7ff5/buildreactionbotframework_960.jpg')
        ])
        .buttons([
            builder.CardAction.openUrl(session, `https://instagram.com/${handle}`, 'Your Profile')
        ]);
}
