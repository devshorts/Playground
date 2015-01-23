var io = require('socket.io');

exports.RealTime = function(server){

    var socketIO = io.listen(server);

    socketIO.set('log level', 1);

    var root = this;

    this.onLogin = function(pushTo){
        root.loginFunction = pushTo;

        return root;
    };

    this.run = function(){
        socketIO.sockets.on('connection', function(socket){
            console.log("connected");

            socket.on("disconnect", function(){
                console.log("disconnect");
            });

            root.loginFunction(root.push);
        });

        return this;
    };

    this.push = function(data) {
        socketIO.sockets.json.emit("data", data);
    }
};