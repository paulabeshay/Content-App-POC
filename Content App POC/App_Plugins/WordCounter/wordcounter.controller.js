var totalCount = 0;

angular.module("umbraco")
    .controller("My.WordCounterApp", function ($scope, editorState, userService, contentResource) {

        console.log("WordCounterApp controller loaded");

        var vm = this;
        vm.CurrentNodeId = editorState.current.id;
        vm.CurrentNodeAlias = editorState.current.contentTypeAlias;

        var counter = contentResource.getById(vm.CurrentNodeId).then(function (node) {
            var properties = node.variants[0].tabs[0].properties;

            vm.propertyWordCount = {};

            var index;
            for (index = 0; index < properties.length; ++index) {
                var words = properties[index].value;

                // Ensure words is a string before processing
                if (typeof words === "string" && words.length != 0) {
                    var wordCount = words.trim().split(/\s+/).length;
                    totalCount += wordCount;
                    vm.propertyWordCount[properties[index].label] = wordCount;
                } else {
                    vm.propertyWordCount[properties[index].label] = 0; // Default to 0 if not a string
                }
            }
            $scope.model.badge = {
                count: totalCount, // the number for the badge - anything non-zero triggers the badge
                type: "warning" // optional: determines the badge color - "warning" = dark yellow, "alert" = red, anything else = blue (matching the top-menu background color)
            };
            totalCount = 0; // Reset totalCount for future use
        });

        var user = userService.getCurrentUser().then(function (user) {
            vm.UserName = user.name;
        });
    });