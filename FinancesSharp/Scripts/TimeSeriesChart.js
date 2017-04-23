/* "data" should be an array of time series, with each item in 
*  this form:
 * {
 *     "SeriesName": NAME_OF_SERIES
 *     "Datapoints": [ DATAPOINT ],
 *     "DrilldownId": id to pass to onDrilldown callback (or null, if not applicable)
 * }
 * Where each datapoint should follow this format:
 * {
 *     "Date": DATE (x axis)
 *     "Value": VALUE (y axis)
 *     "HumanReadableValue": HUMAN_READABLE_VALUE
 * }
 */
function TimeSeriesChart(data, chartSelector, onDrilldown) {
    var margin = {
        left: 260,
        right: 80,
        top: 70,
        bottom: 20
    }
    var totalSize = { width: 1600, height: 800 };
    var chartSize = { width: totalSize.width - (margin.left + margin.right), height: totalSize.height - (margin.top + margin.bottom) };

    var canvas = d3.select(chartSelector)
        .append('svg')
        .attr(totalSize)
        .classed("chart", true);
    var chartArea = canvas.append('g')
        .classed("chartArea", true)
        .attr("transform", "translate(" + margin.left + ", " + margin.top + ")");

    var getDate = function (item) { return item.Date; };
    var getValue = function (item) { return item.Value; };

    var yscale = d3.scale.linear().domain(
        [
            d3.min(data, function (series) { return d3.min(series.Datapoints, getValue); }) * 1.01,
            d3.max(data, function (series) { return d3.max(series.Datapoints, getValue); }) * 1.01
        ]).range([0, chartSize.height]);
    var yAxis = d3.svg.axis();
    yAxis.scale(yscale).orient('left');
    chartArea.append('g')
        .attr('class', 'y axis')
        .call(yAxis);

    var xscale = d3.time.scale().domain(
        [
            new Date(d3.min(data, function (series) { return d3.min(series.Datapoints, getDate); })),
            new Date(d3.max(data, function (series) { return d3.max(series.Datapoints, getDate); }))
        ]).range([0, chartSize.width]);
    var xAxis = d3.svg.axis();
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
           'x1': function (d, i) {
               return xscale(d);
           },
           'y1': function (d) { return 0; },
           'x2': function (d, i) { return xscale(d); },
           'y2': function (d) { return chartSize.height; }
       });

    var colorScale = d3.scale.category20();

    var lineGenerator = d3.svg.line()
        .interpolate("linear")
        .x(function (d) { return xscale(new Date(d.Date)); })
        .y(function (d) { return yscale(d.Value); });

    var tip = new DatapointTooltip({
        offset: { 'x': 0, 'y': -23 },
        rotation: -90,
        centred: true,
        data: function (d, i) {
            return {
                'x': xscale(new Date(d.Date)),
                'y': yscale(d.Value),
                'text': d.HumanReadableValue
            };
        }
    });
    
    var legend = canvas.append("g")
        .attr("class", "legend")
        .attr("transform", "translate(2, " + margin.top + ")");
    legend
        .append("rect")
        .attr("width", 175)
        .attr("height", 250);

    var scatterplot = chartArea.append('g')
       .classed('scatterplot', true);
    scatterplot
       .selectAll('g')
       .data(data)
       .enter()
           .append('g')
           .classed('series', true)
           .each(function (seriesData, seriesIndex) {
               var color = colorScale(seriesIndex);
               var series = d3.select(this)
                   .attr('data-legend', seriesData.SeriesName)
                   .attr('data-legend-color', color);

               legend.append('circle')
                    .attr({
                        'cx': 15,
                        'cy': seriesIndex * 20 + 15,
                        'r': 5,
                        'fill': color
                    });
               var legendText = legend.append('text')
                    .text(seriesData.SeriesName)
                    .attr({
                        'x': 25,
                        'y': seriesIndex * 20 + 20,
                    })
                    .on("mouseover", function (d, i) { setSeriesActive(true); })
                    .on("mouseout", function (d, i) { setSeriesActive(false); });
               legend.select("rect").attr("height", seriesIndex * 20 + 30);

               var setSeriesActive = function (isActive) {
                   scatterplot.classed("active", isActive)
                   series.classed("active", isActive);
                   series.selectAll("circle").transition().attr("r", isActive ? 8 : 5);
                   legend.classed("active", isActive);
                   legendText.classed("active", isActive);
               };

               var line = series
                   .append("path")
                   .classed("line", true)
                   .attr("d", lineGenerator(seriesData.Datapoints))
                   .style("stroke", function (d) { return colorScale(seriesIndex); })
                   .on("mouseover", function (d, i) { setSeriesActive(true); })
                   .on("mouseout", function (d, i) { setSeriesActive(false); });

               var circles = d3.select(this)
                   .append('g')
                   .selectAll('circle')
                   .data(seriesData.Datapoints)
                   .enter()
                       .append('circle')
                       .attr("cx", function (d) { return xscale(new Date(d.Date)); })
                       .attr("cy", function (d) { return yscale(d.Value); })
                       .attr("r", 5)
                       .attr("fill", colorScale(seriesIndex))
                       .on("mouseover", function (d, i) {
                           tip.show(d, i);
                           setSeriesActive(true);
                       })
                       .on("mouseout", function (d, i) {
                           tip.hide(d, i);
                           setSeriesActive(false);
                       });

               if (seriesData.DrilldownId != null) {
                   [line, circles, legendText].forEach(function (set) {
                       set
                           .classed("clickable", true)
                           .on("click", function () {
                               onDrilldown(seriesData.DrilldownId);
                           });
                   });
               }
           });

    tip.appendTo(chartArea);
}