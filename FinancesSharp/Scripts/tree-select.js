var ENTER = 13;
var LEFT = 37;
var UP = 38;
var RIGHT = 39;
var DOWN = 40;

// Elements should be a jQuery set of input elements which should be turned
// into tree selectors. The data should be an array of object. Each object
// must have a 'name' property, which is displayed in the selector, and a
// 'children' property, which should either be an empty array, or an array
// containing more nested objects of the same format.
function TreeSelect(jqSelector, data, settings) {
	var tree = this;

	this.nodes = [];
	this.list = $("<ol></ol>");
	var container = $("<div></div>")
        .addClass("contents")
		.append(this.list);
	this.element = $("<div></div>")
		.addClass("tree-select")
        .append(container)
		.hide();
	this.settings = this.setDefaultSettings(settings);
	if (this.settings.showEmptyOption) {
	    tree.makeChild({
	        name: "",
	        value: "",
            label: "Nothing",
	        children: []
	    });
	}
	data.forEach(function(item) {
	    tree.makeChild(item);
	});
	$("body").append(this.element);

    jqSelector += " input.text";
    $(document).on("focus", jqSelector, function() { 
        tree.clicked($(this).parent());	
    });
    $(document).on("blur", jqSelector, function() {
        tree.blur();
    });
	this.element.find("*")
		.attr("tabindex", 1000)
		.blur(function() { tree.blur(); })
		.focus(function() { tree.focus(); });
}
TreeSelect.prototype.makeChild = function(data) {
    var node = new TreeSelect_Node(data, this.list, this, 0);
    this.nodes.push(node);
};
TreeSelect.prototype.clicked = function(section) {
	var tree = this;
    if (!this.field) {
    	this.field = section.find("input.text");
    	this.hidden = section.find("input.value");
    	this.previousTextField = section.find("input.previous-text");

    	this.field.keyup(function(event) {
    		tree.filter(tree.field.val());
            if (event.which == DOWN) {
                tree.element.find(".info:visible").first().focus();
                event.preventDefault();
            }
    	});
    }
	tree.filter(tree.field.val());
	this.element.find(".highlight").removeClass(".highlight");
	this.collapse();
	tree.focus();
	this.show();	
	this.field.select();
}
TreeSelect.prototype.collapse = function() {
	this.nodes.forEach(function(node) {
		node.recursiveCollapse();
	});
}
TreeSelect.prototype.show = function() {
	var pos = this.field.offset();
	this.element
		.fadeIn()
		.css("left", pos.left)
		.css("top", pos.top + this.field.height());
}
TreeSelect.prototype.select = function (node) {
    this.hidden.val(node.value).trigger('change');
    this.field.val(node.name);
    this.previousTextField.val(node.name);
	this.element.fadeOut();
}
TreeSelect.prototype.filter = function (query) {
    if (query.length == 0 || query.length >= this.settings.minQueryLength) {
        query = query.toLowerCase();
        this.nodes.forEach(function (node) {
            node.filter(query);
        });
    }
}
TreeSelect.prototype.width = function() {
	return 250;
}
TreeSelect.prototype.blur = function() {
	if (this.timer == null) {
		var tree = this;
		this.timer = setTimeout(function() {
		    tree.element.fadeOut();
            if (tree.field.val() == "") {
                tree.hidden.val("");
                tree.previous.val("");
            } else {
		        tree.field.val(tree.previousTextField.val());
            }
		}, 50);
	}
}
TreeSelect.prototype.focus = function() {
	if (this.timer) {
		clearTimeout(this.timer);
		this.timer = null;
	}
}
TreeSelect.prototype.setDefaultSettings = function(input) {
    if (!input) {
        input = {};
    }
    if (!input.hasOwnProperty("minQueryLength")) {
        input.minQueryLength = 2;
    }
    if (!input.hasOwnProperty("showEmptyOption")) {
        input.showEmptyOption = true;
    }
    return input;
}
TreeSelect.prototype.changeContents = function(data) {
    var tree = this;
    data.forEach(function(item) {
        var node = tree.nodes.find(function (n) {
            return n.value == item.value
        });
        if (node) {
            node.updateContents(item);
        } else {
            tree.makeChild(item);
        }
    });
}

// data should be an object with 'name', 'value' and 'children' properties
// Optionally it can include a 'label' property, that is displayed instead
// of 'name' in the dropdown. In this case 'name' is still used in the textbox
// when this item is selected.
function TreeSelect_Node(data, container, tree, padding) {
		var node = this;
		this.name = data.name;
		this.value = data.value;
		this.padding = padding;
		this.label = data.label ? data.label : data.name;

		this.expanded = false;
		this.children = [];
		this.tree = tree;
		this.element = $("<li></li>");
		this.info = $("<span></span>")
			.addClass("info")			
			.css("padding-left", padding * 35 + 5)
			.css("width", tree.width() - padding * 35 + 5)
			.click(function() {
				tree.select(node);
			})
            .keyup(function(event) {
                if (event.which == ENTER) {
                    tree.select(node);
                }
            })
            .keydown(function(event) {
                if (event.which == RIGHT) {
                    node.expand(true);
                }
                if (event.which == LEFT) {
                    if (node.list && node.expanded) {
                        node.collapse();
                    } else {
                        node.element.closest("ol").closest("li").find(".info").first().focus();
                    }
                }
                if (event.which == DOWN) {
                    var next = node.element.find("ol:visible").first().find("li").first();
                    if (!next.length) { 
                        next = node.element.next("li");
                    }
                    if (!next.length) {
                        next = node.element.closest("ol").closest("li").next("li");
                    }
                    next.find(".info").first().focus();
                    event.preventDefault();
                }
                if (event.which == UP) {
                    var previous = node.element.prev("li:visible").first();
                    if (previous.length) {
                        var lastChild = previous.find("li:visible").last();
                        if (lastChild.length) {
                            previous = lastChild;
                        }
                    }
                    if (!previous.length) {
                        previous = node.element.closest("ol").closest("li");
                    }
                    if (previous.length) {
                        previous.find(".info").first().focus();
                    } else {
                        tree.field.focus();
                    }
                    event.preventDefault();
                }
            });
		this.element.append(this.info);
		this.info.append($("<span></span")
			.text(this.label)
			.addClass("name")
		);
		
		if (data.children.length > 0) {
			this.list = $("<ol></ol>").hide();
			data.children.forEach(function (item) {
			    node.makeChild(item);
			});
			this.expander = this.makeExpander();
			this.info.prepend(this.expander);
			this.element.append(this.list);					
		}
		container.append(this.element);
	}
TreeSelect_Node.prototype.filter = function(query) {
	if (query == "") {
		this.collapse();
		this.info.removeClass("highlight");
		this.element.slideDown();
		return;
	}

	var anyChildren = false;
	if (this.list) {
		this.children.forEach(function(child) {
			if (child.filter(query)) {
				anyChildren = true;
			}
		});
		if (anyChildren) {
			this.expand(false);
		} else {
			this.collapse();
		}
	}

	var myself = this.name.toLowerCase().indexOf(query) >= 0;
	if (myself) {
		this.info.addClass("highlight");
	} else {
		this.info.removeClass("highlight");
	}

	var visible = myself || anyChildren;
	if (visible) {
		this.element.slideDown();
	} else {
		this.element.slideUp();
	}
	return visible;
}
TreeSelect_Node.prototype.expand = function(expandChildren) {
	if (expandChildren) {
		this.children.forEach(function(child) {
			if (!child.expanded) {
				child.element.show();
			}
		});
	}
	this.list.slideDown();
	this.expanded = true;
	this.expander.attr("src", "/Content/minus.png");
}
TreeSelect_Node.prototype.collapse = function() {
	if (this.list) {
		this.list.slideUp();
	}
	this.expanded = false;
	if (this.expander) {
	    this.expander.attr("src", "/Content/plus.png");
	}
}
TreeSelect_Node.prototype.recursiveCollapse = function() {
	this.collapse(false);
	this.children.forEach(function(child) {
		child.recursiveCollapse();
	});
}
TreeSelect_Node.prototype.makeExpander = function() {
	var node = this;
	var e = $("<img></img>")
		.addClass("expander")
		.attr("src", "/Content/minus.png")
		.click(function(e) {
			e.stopPropagation();
			if (node.expanded) {
				node.collapse();
			} else {
				node.expand(true);
			}
		});
	return e;
}
TreeSelect_Node.prototype.makeChild = function (data) {
    this.children.push(new TreeSelect_Node(data, this.list, this.tree, this.padding + 1));
}
TreeSelect_Node.prototype.updateContents = function (data) {
    var node = this;
    data.children.forEach(function (item) {
        var child = node.children.find(function (n) {
            return n.value == item.value;
        });
        if (child) {
            child.updateContents(item);
        } else {
            node.makeChild(item);
        }
    });
}