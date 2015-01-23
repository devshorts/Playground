var openurl = require("openurl");

var Server = require("./src/server").Server,
    RealTime = require("./src/realtime").RealTime,
    InstagramRss = require("./src/instagramRss").InstagramRss;

var App = function(){

    var config = require('./config.json');

    var rss = new InstagramRss(config.tag, config.take);

    var server = new Server();

    var realtime = {};

    this.run = function(){
        runOnTimer(config.interval);

        realtime = new RealTime(server.start()).onLogin(rss.query).run();
    };

    function runOnTimer(interval){
        setInterval(function(){
            rss.query(realtime.push)
        }, interval * 1000);
    }
};


new App().run();

openurl.open('http://localhost:3000');