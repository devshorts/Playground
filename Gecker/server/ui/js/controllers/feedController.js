function feedController($scope, realtime, $http){
    $scope.feed = [];

    realtime.registerRssPush(function (data) {
        console.log("got data");

        var map = {};

        _.forEach($scope.feed, function (item){
            map[item.link] = true;
        });

        _.forEach(data, function (item) {
            if(!map.hasOwnProperty(item.link)){
                $scope.feed.unshift(item);
            }
        });

        $scope.$apply();
    });
}