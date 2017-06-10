function HorizontalBarChart(data, chartSelector, onDrilldown) {    
    var margin = {
        left: 200,
        right: 80,
        top: 30,
        bottom : 0
    }
    var totalSize = { width: 1600, height: data.length * 60 + margin.top + margin.bottom };
    var chartSize = { width: totalSize.width - (margin.left + margin.right), height: totalSize.height - (margin.top + margin.bottom) };

    var canvas = d3.select(chartSelector)
        .append('svg')
        .attr(totalSize)
        .classed("chart", true);
    var chartArea = canvas.append('g')
        .classed("chartArea", true)
        .attr("transform", "translate(" + margin.left + ", " + margin.top + ")");

    var yscale = d3.scale.linear().domain([0, data.length]).range([0, chartSize.height]);
    var rowHeight = yscale(1) - yscale(0);
    var barThickness = rowHeight - 10;

    var makeYAxis = function(tickSize, offset, tickFormat, isPale) {
        var yAxis = d3.svg.axis();
        yAxis
            .orient('left').scale(yscale).tickSize(tickSize)
            .tickFormat(tickFormat)
            .tickValues(d3.range(data.length));
        chartArea.append('g')
            .attr('class', 'y axis')
            .classed("pale", isPale)
            .call(yAxis)
            .call(function(selection) {
                selection.selectAll('.tick text').attr('transform', 'translate(0, ' + (rowHeight / 2 + offset) + ')');
            });
    }
    makeYAxis(2, -12, function(d, i) { return data[i].MainLabel }, false);
    makeYAxis(0,  10, function(d, i) { return data[i].SubLabel  }, true);

    var xscale = d3.scale.linear()
        .domain([0, d3.max(data, function(item) { return item.Value })])
        .range([0, chartSize.width]);
    var	xAxis = d3.svg.axis();
    xAxis.scale(xscale).orient('top');
    chartArea.append('g')
        .attr('class', 'x axis')
        .call(xAxis);

    var grids = chartArea.append('g')
       .attr("class", "grid")
       .selectAll('line')
       .data(xscale.ticks())
       .enter()
       .append('line')
       .attr({
           'x1': function(d,i) {
               return xscale(d);
           },
           'y1': function(d) { return 0; },
           'x2': function(d,i) { return xscale(d); },
           'y2': function(d) { return chartSize.height; }
       });

    var tip = new DatapointTooltip({
        offset: { 'x': 23, 'y': 5 }, 
        rotation: 0, 
        centred: false,
        data: function (d, i) {
            return {
                'x': xscale(d.Value),
                'y': yscale(i) + barThickness / 2,
                'text': d.HumanReadableValue
            };
        }
    });
    
    chartArea.append('g')
        .attr('class', 'bars')
        .selectAll('rect')
        .data(data)
        .enter()
            .append('rect')
            .attr('height', barThickness)
            .attr('width', function (d) { return xscale(d.Value); })
            .attr({ 'x': 0, 'y': function (d, i) { return yscale(i); } })
            .on('mouseover', tip.show)
            .on('mouseout', tip.hide)
            .filter(function (d) { return d.DrilldownId; })
            .classed("clickable", true)
            .on("click", function (d, i) {
                onDrilldown(d.DrilldownId);
            });

    tip.appendTo(chartArea);
}