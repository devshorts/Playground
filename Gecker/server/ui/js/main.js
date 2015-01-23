function App(){
    this.run = function(app){
        new ServiceInitializer().initServices(app);
        new Directives().initDirectives(app);

        applyConfigs(app);
    };

    function applyConfigs(app){
        app.config(function($stateProvider, $urlRouterProvider){

            $urlRouterProvider.otherwise("/");

            $stateProvider.state('main', {
                url:"/",
                templateUrl: "partials/feed.html",
                controller: feedController
            })
        });
    }
}