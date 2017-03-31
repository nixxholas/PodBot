require('dotenv').config()

const http = require('http');
const express = require('express');
const restify = require('restify');
const builder = require('botbuilder');
const ig = require('instagram-node').instagram();

//=========================================================
// External API Setup
//=========================================================

ig.use({ client_id: process.env.INSTAGRAM_CLIENT_ID,
         client_secret: process.env.INSTAGRAM_CLIENT_SECRET });

//=========================================================
// Bot Setup
//=========================================================

// Setup the main process
const app = express();

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

exports.authorize_user = function(req, res) {
  res.redirect(api.get_authorization_url(redirect_uri, { scope: ['likes'], state: 'a state' }));
};
 
exports.handleauth = function(req, res) {
  api.authorize_user(req.query.code, redirect_uri, function(err, result) {
    if (err) {
      console.log(err.body);
      res.send("Didn't work");
    } else {
      console.log('Yay! Access token is ' + result.access_token);
      res.send('You made it!!');
    }
  });
};
 
// This is where you would initially send users to authorize 
app.get('/authorize_user', exports.authorize_user);
// This is your redirect URI 
app.get('/handleauth', exports.handleauth);
 
http.createServer(app).listen(app.get('port'), function(){
  console.log("Express server listening on port " + app.get('port'));
});

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

bot.dialog('/setupprofile', [
    function (session) {
        // Expect the bot to receive a reply from the user, thus use Prompts
        builder.Prompts.text(session, 'Hi! What is your handle?');
    },
    function (session, results) {
        // Retrieve the user's response
        session.userData.name = results.response;

        var card = createThumbnailCard(session, session.userData.name);

        
var access_token = "ACCESS_TOKEN";   
var user_id = "USER_ID";
var url = "https://api.instagram.com/v1/users/"+user_id+"?access_token="+access_token+"&callback=?";
$.getJSON(url, function(data) {
    $("body").append("<img src='"+data.data.profile_picture+"' />");
});
        // attach the card to the reply message
        var msg = new builder.Message(session).addAttachment(card);
        session.send(msg);
        
        //session.endDialog();
    }
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
