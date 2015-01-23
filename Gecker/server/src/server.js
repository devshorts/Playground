var express = require('express');
var http = require('http');
var app = express();
exports.Server = function(){
    var hostRoot = __dirname + '/../ui';

    console.log(hostRoot);

    app.use(express.bodyParser());
    app.use(express.methodOverride());
    app.use(app.router);
    app.use(express.static(hostRoot));
    app.use(express.errorHandler());

    this.start = function(){
        var server = http.createServer(app);

        var port = process.env.PORT || 3000;
        server.listen(port);

        console.log("listneing on " + port);

        return server;
    };

    this.addRoutes = function(callback){
        callback(app);
    };
};