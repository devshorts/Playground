function Directives(){
    this.initDirectives = function(app){
        app.directive('instagram', instagram)
    };

    function instagram(){
        return {
            restrict: 'E',
            scope: {
                data:"="
            },
            templateUrl: 'partials/directives/instagram-directive.html',
            link: function (scope, element, attrs){
                var img = $(element).find("img")[0];
                var txt = $(element).find(".image-text")[0];

                $(img).bind("load", function(event){
                    $(img).css("opacity", 1);
                    $(txt).css("opacity", 1);
                });
            }
        };
    }
}