(function ($) {
    Runner = {};



    Runner.startRun = function () {
        console.log("register click");
        var $score = $("#run-score");
        var $results = $("#run-results");

        $results.text($score.val());

        $("#start-run").click(function () {

            PackSimulator.getNextPack(function (results) {

                console.log(results);

                var oldText = $results.text();
                var newScore = $score.val();
                $results.text(oldText + "\n" + newScore);
            });
        });
    };

    Runner.startRun();

    return Runner;


})(jQuery);