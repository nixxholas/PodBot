require('dotenv').config()

const http = require('http');
const express = require('express');
const restify = require('restify');
const builder = require('botbuilder');
const passport = require('passport')
  , InstagramStrategy = require('passport-instagram').Strategy
  , InstagramTokenStrategy = require('passport-instagram-token');
const instagram = require('node-instagram');

//=========================================================
// Bot Setup
//=========================================================

// Define some entities to help with storage
var LikeList = new Array();
var UserList = new Array();

// Setup Restify Server
const server = restify.createServer();
server.listen(process.env.port || 3978, function () {
   console.log('%s listening to %s', server.name, server.url); 
});
  
// Create chat bot
const connector = new builder.ChatConnector({
    appId: process.env.MICROSOFT_APP_ID,
    appPassword: process.env.MICROSOFT_APP_PASSWORD
});
const bot = new builder.UniversalBot(connector);

//=========================================================
// Passport Instagram OAuth Setup
//=========================================================

passport.use(new InstagramTokenStrategy({
    clientID: process.env.INSTAGRAM_CLIENT_ID,
    clientSecret: process.env.INSTAGRAM_CLIENT_SECRET,
    passReqToCallback: true
}, function(req, accessToken, refreshToken, profile, next) {
    User.findOrCreate({'instagram.id': profile.id}, function(error, user) {
        return next(error, user);
    });
}));

server.use(passport.initialize());
server.use(passport.session());

//=========================================================
// Bot API
//=========================================================

server.get('/auth/instagram', passport.authenticate('instagram-token'));
server.post('/api/messages', connector.listen());

//=========================================================
// Bots Dialogs
//=========================================================

bot.dialog('/', new builder.IntentDialog()
    .matches(/^set name/i, builder.DialogAction.beginDialog('/profile'))
    .matches(/^quit/i, builder.DialogAction.endDialog())
    .matches(/^add post/i, builder.DialogAction.beginDialog('/addpost'))
    .matches(/^listall/i, builder.DialogAction.beginDialog('/listall'))
    .matches(/^(\?|help)/i, builder.DialogAction.beginDialog('/help'))
    .onDefault((session) => {
        session.sendTyping();
        if (!session.userData.name) {
            session.beginDialog('/profile');
        } else {
            session.send(`Hm, I did not understand you ${session.userData.name} :(`);
        }
    }));
bot.dialog('/profile',  [
    (session) => {
        session.sendTyping();
        if (session.userData.name) {
            builder.Prompts.text(session, 'What would you like to change it to?');
        } else {
            builder.Prompts.text(session, 'Hey! What is your instagram handle?');
        }
    },
    (session, results, next) => {
        session.sendTyping();
        if (results.response) {
            session.userData.handle = results.response;
            builder.Prompts.text(session, "What's your name?");
        } else {
            next();
        }
    },
    (session, results) => {
        session.sendTyping();
        session.userData.name = results.response;

        // Passport JS Authentication

        var User = new User(session.userData.handle ,session.userData.name);

        session.send(`Hello ${session.userData.name}, it\'s great to meet you. I\'m PodBot.`);
        session.endDialog();
    }
]);
bot.dialog('/listall',  [
    (session) => {
        session.sendTyping();
        session.send('Here\'s all the posts that are posted today:');
        session.endDialog();
    }
]);
bot.dialog('/addpost',  [
    (session) => {
        session.sendTyping();
        if (session.userData.name) { // If the user has setup a handle
            builder.Prompts.text(session, 'Simply toss in your post URL.');
        } else {
            session.beginDialog('/profile');
        }
    },
    (session, results) => {
        session.sendTyping();
        session.userData.name = results.response;
        session.send(`Hello ${session.userData.name}, it\'s great to meet you. I\'m PodBot.`);
        session.endDialog();
    }
]);
bot.dialog('/help', [
    (session) => {
        session.sendTyping();
        session.send('PodBot Commands: \n' +
        '- ');
        session.endDialog();
    }
]);