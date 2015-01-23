var request = require('request');
var _ = require('underscore')._;
var xml2js = require("xml2js");
var Instagram = require("instagram-node-lib");

exports.InstagramRss = function(tag, takeAmount){
    var options = {
        host: "http://instagram.com/tags/" + tag + "/feed/recent.rss",
        method: 'GET'
    };

    this.query = function(callback){
        function extractor(body){
            var parser = xml2js.Parser();

            return parser.parseString(body, function(err, r){
                var items =
                    _.chain(r.rss.channel[0].item)
                        .map(function(element){
                            return {
                                link : element.link[0],
                                title: element.title[0]
                            }
                        })
                        .take(takeAmount)
                        .value();

                callback(items);
            });
        }

        request(options.host, function (error, response, body) {
            if (!error && response.statusCode == 200) {
                extractor(body);
            }
        })
    };
};