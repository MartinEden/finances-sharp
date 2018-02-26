$(function () {
    $('.datepicker input').datepicker({ dateFormat: 'dd/mm/yy' });
    var selectors = [];
    
    // Setup category editing
    $.ajax("/Category/All", {
        dataType: "json",
        success: function (categories) {
            selectors.push(new TreeSelect(".category-box-no-new", categories));
            var selector = new TreeSelect(".category-box", categories);
            selectors.push(selector);
            var button = $("<button></button>").text("Add new category");
            selector.element.append(button);
            button.click(function() {
                var panel = $("#new-category");
                panel.show();                
                $("#panels").fadeIn();
                panel.find("input.name").focus();
            });
        }
    });
    $(".fancy-box.category-box")
        .find("input.value")
        .change(function () {
            var input = $(this);
            var id = input.parent().data("id");
            $.ajax("/Category/Change/" + id, {
                data: { "categoryId": input.val() },
                type: 'POST',
            });
        });

    // Setup name editing
    $(".fancy-box-name").find("input")
        .on('keyup', function (event) {
            if (event.which == '13') {
                $(this).blur();
            }
        })
        .blur(function () {
            var input = $(this);
            var id = input.parent().data("id");
            $.ajax("/Home/ChangeName/" + id, {
                data: { "name": input.val() },
                type: 'POST',
            });
        });

    // Setup card editing
    $(".fancy-box.card-box")
        .find("input.value")
        .on('keyup', function (event) {
            if (event.which == '13') {
                $(this).blur();
            }
        })
        .blur(function() {
            var input = $(this);
            var cardNumber = input.parent().data("cardNumber");
            $.ajax("/Card/ChangeOwner/" + cardNumber, {
                data: { "personName": input.val() },
                type: 'POST',
            });
        })


    var refreshCategorySelectors = function () {
        $.ajax("/Category/All", {
            dataType: "json",
            success: function (categories) {
                selectors.forEach(function (selector) {
                    selector.changeContents(categories);
                });
            },
        });
    };

    $("#new-category form").submit(function (e) {
        var form = $(this);
        e.preventDefault();
        $.ajax(form.attr("action"), {
            dataType: "json",
            method: "post",
            data: form.serialize(),
            success: function () { 
                refreshCategorySelectors();
            },
        });
        $("#panels").fadeOut();
    });
});