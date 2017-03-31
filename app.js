require('dotenv').config()

const http = require('http');
const express = require('express');
const restify = require('restify');
const builder = require('botbuilder');
const passport = require('passport')
  , InstagramStrategy = require('passport-instagram').Strategy;

//=========================================================
// Bot Setup
//=========================================================

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
// Passport Instagram OAuth Setup
//=========================================================

server.use(passport.initialize());
server.use(passport.session());

//=========================================================
// Bots Dialogs
//=========================================================

bot.dialog('/', new builder.IntentDialog()
    .matches(/^set name/i, builder.DialogAction.beginDialog('/profile'))
    .matches(/^quit/i, builder.DialogAction.endDialog())
    .matches(/^what.+(best|favorite).+IDE/i, builder.DialogAction.beginDialog('/sourcelair'))
    .matches(/^(\?|help)/i, builder.DialogAction.beginDialog('/help'))
    .onDefault((session) => {
        if (!session.userData.name) {
            session.beginDialog('/profile');
        } else {
            session.send(`Hm, I did not understand you ${session.userData.name} :(`);
        }
    }));
bot.dialog('/profile',  [
    (session) => {
        if (session.userData.name) {
            builder.Prompts.text(session, 'What would you like to change it to?');
        } else {
            builder.Prompts.text(session, 'Hi! What is your name?');
        }
    },
    (session, results) => {
        session.userData.name = results.response;
        session.send(`Hello ${session.userData.name}, it\'s great to meet you. I\'m PodBot.`);
        session.endDialog();
    }
]);
bot.dialog('/sourcelair',  [
    (session) => {
        session.send('It\'s SourceLair of course <3');
        session.endDialog();
    }
]);
bot.dialog('/help', [
    (session) => {
        session.send('You can always ask me some questions, for example what is my favorite IDE');
        session.endDialog();
    }
]);