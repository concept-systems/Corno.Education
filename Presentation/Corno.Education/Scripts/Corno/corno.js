/*var gridName = 'grid';*/

function index(dataItem, gridName = "grid") {
	//alert(gridName);
	if (undefined === gridName)
		gridName = "grid";
	var data = $('#' + gridName).data("kendoGrid").dataSource.data();
	return data.indexOf(dataItem);
}

//function onDataBound(e) {
//	// alert("DataBound");
//	var grid = $("#" + gridName).data("kendoGrid");
//	for (var i = 0; i < grid.columns.length; i++) {
//		grid.autoFitColumn(i);
//	}
//}

//function onChange(arg) {
//	// alert('#' + gridName);
//	var grid = $('#' + gridName).data("kendoGrid");

//	var data = grid.dataSource.view();
//	console.log(arg);
//}

//function onEdit(e) {
//    $("#ItemId").hide();
//    $("label[for='ItemId']").hide();
//}

function error_handler(e) {
	if (e.errors) {
		var message = "";//"Errors:<br>";
		var counter = 0;
		$.each(e.errors, function (key, value) {
			if ('errors' in value) {
				$.each(value.errors, function () {
					message += (++counter) + ". " + this + "<br>";
				});
			}
		});
		bootbox.alert({ title: "Errors", message: message });
	}
}

//function error_handler1(e) {
//	if (e.errors) {
//		var message = "";//"Errors:<br>";
//		var counter = 0;
//		$.each(e.errors, function (key, value) {
//			if ('errors' in value) {
//				$.each(value.errors, function () {
//					message += (++counter) + ". " + this + "<br>";
//				});
//			}
//		});
//		bootbox.alert({ title: "Errors", message: message });
//	}
//}