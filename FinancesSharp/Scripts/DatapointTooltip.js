function DatapointTooltip(settings) {
    var offset = settings.offset;
    var rotation = settings.rotation;
    var dataCallback = settings.data;
    var centred = settings.centred;

    var me = this;
    this.appendTo = function (container) {
        me.group = container.append('g')
            .classed('datapoint-tooltip', true)
        me.box = me.group.append('g');
        me.rect = me.box.append('rect')
            .attr('height', 20)
            .attr({ 'x': -5, 'y': -15 });
        me.text = me.box.append("text");
        me.divot = me.group.append("polygon")
            .attr("points", "0,0 15,-6 15,+6")
            .attr("transform", "rotate(" + rotation + ")");
    }
    this.show = function (d, i) {
        var data = dataCallback(d, i);
        me.text.text(data.text);
        var bbox = me.text.node().getBBox();
        var width = bbox.width + 10;
        me.rect.attr('width', width);

        if (centred) {
            var centreX = -width / 2;
        } else {
            var centreX = 0;
        }

        me.group
            .transition()
            .style("opacity", 1)
            .attr("transform", "translate(" + data.x + ", " + data.y + ")");
        me.box
            .transition()
            .attr("transform", "translate(" + (offset.x + centreX) + ", " + offset.y + ")");
    }
    this.hide = function (d, i) {
        me.group.transition().style("opacity", 0);
    }
}