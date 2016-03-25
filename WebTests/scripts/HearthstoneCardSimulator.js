(function () {
    var f = 0;
    var h = 400;
    var e = [];
    $(".pack-slot").each(function (l) {
        var i = $(this);
        var k = parseInt(i.attr("data-id"));
        var m = parseInt(i.attr("data-rarity"));
        var j = parseInt(i.attr("data-isgolden"));
        if (!i.hasClass("revealed")) {
            e.push(m + "-" + j + "_" + k + "_" + l);
        }
    });
    e.sort();
    for (var g = 0; g < e.length; g++) {
        var d = e[g];
        (function (j, k) {
            var j = j.split(/_/);
            var o = j[0].split(/-/)[0];
            var l = j[0].split(/-/)[1];
            var m = j[1];
            var n = j[2];
            var i = $(".pack-slot").eq(n);
            //setTimeout(function() {
            //if (o >= 4 || o == 3 && l == 1) {
            //i.addClass("preview");
            //setTimeout(function() {
            //i.removeClass("preview")
            //}, 250)
            //} else {
            i.trigger("mouseup");
            //}
            //}, k + h)
        })(d, f);
        f += h;
    }
})();