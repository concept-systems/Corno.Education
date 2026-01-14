// Write your Javascript code.

window.kendoReady = function (callback) {
    $(function () {
        if (typeof kendo !== "undefined") {
            callback();
        } else {
            console.error("Kendo is not loaded.");
        }
    });
};

$(function () {
    if (typeof kendo !== "undefined") {
        // Safe to use Kendo here
        console.log("Kendo is loaded");
    } else {
        console.error("Kendo is still undefined");
    }
});

//function renderStatusBadge(status) {
//    let backgroundColor = 'transparent';
//    let textColor = 'black';

//    if (status === 'Active') {
//        backgroundColor = 'green';
//        textColor = 'white';
//    } else if (status === 'Deleted') {
//        backgroundColor = 'red';
//        textColor = 'white';
//    }

//    return `<span class='badge' style='
//        background-color: ${backgroundColor};
//        color: ${textColor};
//        text-align: center;
//        display: inline-block;
//      '>${status}</span>`;
//}

function renderStatusBadge(status) {
    let backgroundColor = 'transparent';
    let textColor = 'black';

    switch (status) {
        case 'Active':
            //backgroundColor = '#28a745'; // green
            //textColor = 'white';
            backgroundColor = '#90ee90'; // light green
            textColor = 'black';
            break;
        case 'Printed':
            backgroundColor = '#007bff'; // blue
            textColor = 'white';
            break;
        case 'Bent':
            backgroundColor = '#6f42c1'; // purple
            textColor = 'white';
            break;
        case 'Sorted':
            backgroundColor = '#17a2b8'; // teal
            textColor = 'white';
            break;
        case 'SubAssembled':
            backgroundColor = '#fd7e14'; // orange
            textColor = 'white';
            break;
        case 'Packed':
            backgroundColor = '#20c997'; // greenish-teal
            textColor = 'white';
            break;
        case 'Pallet In':
            backgroundColor = '#6610f2'; // indigo
            textColor = 'white';
            break;
        case 'Rack In':
            backgroundColor = '#6c757d'; // gray
            textColor = 'white';
            break;
        case 'Rack Out':
            backgroundColor = '#343a40'; // dark gray
            textColor = 'white';
            break;
        case 'Dispatch':
            backgroundColor = '#ffc107'; // yellow
            textColor = 'black';
            break;
        case 'Loaded':
            backgroundColor = '#007bff'; // blue
            textColor = 'white';
            break;
        case 'Unload':
            backgroundColor = '#17a2b8'; // cyan
            textColor = 'white';
            break;
        case 'InProcess':
            backgroundColor = '#fd7e14'; // orange
            textColor = 'white';
            break;
        case 'Deleted':
            backgroundColor = '#dc3545'; // red
            textColor = 'white';
            break;
        case 'Approved':
            console.log("In Approved")
            backgroundColor = '#198754'; // Bootstrap green
            textColor = 'white';
            break;
        case 'Rejected':
            //console.log("In Rejected")
            backgroundColor = '#dc3545'; // Bootstrap red
            textColor = 'white';
            break;
        default:
            //backgroundColor = '#f8f9fa'; // light gray
            textColor = '#212529'; // dark text
            break;
    }

    return `<span class='badge' style='
        background-color: ${backgroundColor};
        color: ${textColor};
        text-align: center;
        display: inline-block;
        padding: 4px 8px;
        border-radius: 4px;
        font-weight: 500;
        font-size: 0.85em;
      '>${status}</span>`;
}



function enhanceStatusBadges() {
    console.log("enhanceStatusBadges")
    $(".status-badge").each(function () {
        const status = $(this).data("status");
        $(this).replaceWith(renderStatusBadge(status));
    });
}

////Cell size auto grid responsive mobile to pc  
//$(function () {
//    const grid = $("#grid").data("kendoGrid");
//    console.log("enhanceStatusBadges : " + grid);
//    if (grid) {
//        grid.bind("dataBound", function () {
//            enhanceStatusBadges();
//        });
//    }
//});

function autoFitGridColumns(gridId) {
    var grid = $("#" + gridId).data("kendoGrid");
    if (!grid) return;

    // Temporarily set table-layout to auto
    $(grid.table).css("table-layout", "auto");

    grid.columns.forEach((col, index) => {
        grid.autoFitColumn(index);
    });
}

function onGridDataBound(e) {
    var gridElement = e.sender.element; // This is a jQuery object
    var gridId = gridElement.attr("id"); // This gives you the ID of the grid

    console.log("Grid : " + gridId);

    enhanceStatusBadges();
    autoFitGridColumns(gridId);

    //var grid = $("#grid").data("kendoGrid");
    //if (grid) {
    //    // Optional: hide columns if needed
    //    grid.hideColumn("Code");
    //    grid.hideColumn("Description");
    //}

    //// Center-align cells
    //$(".k-grid td").css("text-align", "center");
    //$(".k-grid-header th").addClass("k-text-center !k-justify-content-center");

    //// Wrap grid in scrollable container if not already wrapped
    //var $gridElement = $("#grid");
    //if ($gridElement.length && !$gridElement.parent().hasClass("k-grid-wrapper")) {
    //    $gridElement.wrap('<div class="k-grid-wrapper" style="overflow-x:auto;"></div>');
    //}
}

//function onPdfExport(e) {
//    e.preventDefault(); // Cancel default export

//    var grid = $("#grid").data("kendoGrid");
//    var originalPageSize = grid.dataSource.pageSize();

//    // Fetch all data manually
//    grid.dataSource.query({
//        page: 1,
//        pageSize: grid.dataSource.total(), // Fetch all records
//        sort: grid.dataSource.sort()
//    }).then(function () {
//        // Export after data is loaded
//        grid.saveAsPDF();

//        // Restore original page size without triggering another read
//        setTimeout(function () {
//            grid.dataSource.query({
//                page: 1,
//                pageSize: originalPageSize,
//                sort: grid.dataSource.sort()
//            });
//        }, 1000);
//    });
//}



function onPdfExport(e) {
    var grid = $("#grid").data("kendoGrid");

    // Hide Actions column
    var actionsColumnIndex = grid.thead.find("th:contains('Actions')").index();
    grid.hideColumn(actionsColumnIndex);

    // Hide filter row
    $(".k-filter-row").hide();

    // Hide column menu icons
    $(".k-header-column-menu").hide();

    // Restore everything after export
    setTimeout(function () {
        grid.showColumn(actionsColumnIndex);
        $(".k-filter-row").show();
        $(".k-header-column-menu").show();
    }, 1000);
}

function onGridError(e) {
    console.log("Grid error:", e);

    if (e.errors) {
        let message = "";// "Errors:\n\n";
        $.each(e.errors, function (key, value) {
            //console.log("Value : " + JSON.stringify(value));
            if (value && value.errors) {
                //console.log("value.errors : " + JSON.stringify(value.errors));
                $.each(value.errors, function (_, error) {
                    //console.log("error: " + error);
                    message += error + "\n";
                });
            }
            else if (Array.isArray(value)) {
                // Sometimes errors come as an array of strings
                value.forEach(function (error) {
                    message += error + "\n";
                });
            }
        });
        bootbox.alert(message.trim());
    }
}

