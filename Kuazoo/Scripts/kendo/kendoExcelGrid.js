(function ($) {
    var kendo = window.kendo;

    var ExcelGrid = kendo.ui.Grid.extend({
        init: function (element, options) {
            var that = this;
            if (options.excel) {
                // If the excelCssClass is not defined, then set a default image.
                options.excel.cssClass = options.excel.cssClass || "k-i-expand";

                // Add the excel toolbar button.
                options.toolbar = $.merge([
                    {
                        name: "excel",
                        template: kendo.format("<a class='k-button k-button-icontext k-grid-excel' title='Export to Excel'><div class='{0} k-icon'></div>Export</a>", options.excel.cssClass)
                    }
                ], options.toolbar || []);
            }

            // Initialize the grid.
            kendo.ui.Grid.fn.init.call(that, element, options);

            // Add an event handler for the excel button.
            $(element).on("click", ".k-grid-excel", { sender: that }, function (e) {
                e.data.sender.exportToExcel();
            });
        },

        options: {
            name: "ExcelGrid"
        },

        exportToExcel: function () {
            var that = this;
            // Define the data to be sent to the server to create the spreadsheet.
            var _model = JSON.stringify(that.columns);
            _model = _model.replace("<a class='texthref' href='/Sales/MerchantDetails/#=merchantid#'>#=name#</a>", "#=name#");
            _model = _model.replace("<a class='texthref' href='/Admin/StoreDetails/#=merchantid#'>#=name#</a>", "#=name#");
            data = {
                model: _model,
                data: JSON.stringify(that.dataSource.data().toJSON()),
                title: that.options.excel.title
            };

            // Create the spreadsheet.
            $.post(that.options.excel.createUrl, data, function () {
                // Download the spreadsheet.
                window.location.replace(kendo.format("{0}?title={1}",
                    that.options.excel.downloadUrl,
                    that.options.excel.title));
            });
        }
    });

    kendo.ui.plugin(ExcelGrid);
})(jQuery);