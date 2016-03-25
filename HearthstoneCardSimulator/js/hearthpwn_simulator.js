// ==UserScript==
// @name         Getpack results
// @namespace    http://tampermonkey.net/
// @version      0.1
// @description  try to take over the world!
// @author       You
// @match        http://www.hearthpwn.com/packs/simulator/1-hearthpwn-wild-pack
// @grant        none
// ==/UserScript==
/* jshint -W097 */
//'use strict';

(function($) {

    PackSimulator = {};
    var packUrl = "http://www.hearthpwn.com/packs/simulator/1-hearthpwn-wild-pack";

    packResults = function($html){
        var pack = {
            score: parseInt($html.closest(".pack-score").attr("data-score")),
            card: []
        };

        $html.closet(".pack-results .pack-slot").each(function () {
            var card = $(this);

            var temp = {
                name: card.find(".card-front").attr('href'),
                id: card.attr("data-id"),
                isgolden: card.attr("data-isgolden"),
                rarity: card.attr("data-rarity")
            };

            pack.card.push(temp);

        });

        return pack;
    }

    PackSimulator.getNextPack = function (callback) {
        $.ajax({
            url: packUrl,
            type: "GET",
            crossDomain: true,
            dataType: "HTML"
        }).done(function (result) {
            var $html = $.parseHTML(result);
            var results = packResults($html);

            callback(results);
        });
    }

    return PackSimulator;
})(jQuery);


//function packResults() {
//    var pack = {
//        score: parseInt($(".pack-score").attr("data-score")),
//        card: []
//    };

//    $(".pack-results .pack-slot").each(function () {
//        var card = $(this);

//        var temp = {
//            name: card.find(".card-front").attr('href'),
//            id: card.attr("data-id"),
//            isgolden: card.attr("data-isgolden"),
//            rarity: card.attr("data-rarity")
//        };
        
//        pack.card.push(temp);

//    });

//    return pack;
//}

//$(function () {
//    var pack = packResults();
//    console.log(pack);
//    var displayPack = "";
//    var hasLegendary = false;
//    for (var i = 0; i < pack.card.length; i++) {
//        var rarity = "";
//        switch (pack.card[i].rarity) {
//            case "1":
//                rarity = '<span style="background-color:red;display:inline-block;padding:5px;color:black;">Classic</span> ';
//                break;
//            case "2":
//                rarity = '<span style="background-color:silver;display:inline-block;padding:5px;color:black;">Common</span> ';
//                break;
//            case "3":
//                rarity = '<span style="background-color:rgb(36,113,231);display:inline-block;padding:5px;color:black;">Rare</span> ';
//                break;
//            case "4":
//                rarity = '<span style="background-color:#B131D9;display:inline-block;padding:5px;color:black;">Epic</span> ';
//                break;
//            case "5":
//                hasLegendary = true;
//                rarity = '<span style="background-color#rgb(253,143,12);display:inline-block;padding:5px;color:black;">Legendary</span> ';
//                break;
//        }
//        var golden = '';
//        if(pack.card[i].isgolden == '1') {
//            var golden = '<span style="background-color:gold;display:inline-block;padding:5px;color:black;">Golden</span> ';
//        }

//        var cardRarity = golden + rarity;
//        displayPack += cardRarity + ": " + pack['card'][i]['name'] + "<br /><br />";
//    }

//    if(pack.score > 100000) {
//        alert("good pack");
//        return false;
//    }
//    else {
//        location.reload();
//        $("html").html(displayPack + "<br /><h2>Score: " + pack['score'] + "</h2>");
//    }
//});