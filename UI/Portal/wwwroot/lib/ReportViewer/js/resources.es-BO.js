﻿; (function(trv, $) {
	"use strict";

	var sr = {
		controllerNotInitialized: 'Controller is not initialized.',
		noReportInstance: 'No report instance.',
		missingTemplate: 'ReportViewer template is missing. Please specify the templateUrl in the options.',
		noReport: 'No report.',
		noReportDocument: 'No report document.',
		invalidParameter: 'Please input a valid value.',
		invalidDateTimeValue: 'Please input a valid date.',
		parameterIsEmpty: 'Parameter value cannot be empty.',
		cannotValidateType: 'Cannot validate parameter of type {type}.',
		loadingFormats: 'Cargando...',
		loadingReport: 'Cargando Reporte...',
		preparingDownload: 'Preparing document to download. Please wait...',
		preparingPrint: 'Preparing document to print. Please wait...',
		errorLoadingTemplates: 'Error loading the report viewer\'s templates.',
		loadingReportPagesInProgress: '{0} páginas cargadas hasta ahora ...',
		loadedReportPagesComplete: 'Terminado. Total {0} páginas cargadas.',
		noPageToDisplay: "No hay páginas para mostrar.",
		errorDeletingReportInstance: 'Error deleting report instance: {0}',
		errorRegisteringViewer: 'Error registering the viewer with the service.',
		noServiceClient: 'No serviceClient has been specified for this controller.',
		errorRegisteringClientInstance: 'Error registering client instance',
		errorCreatingReportInstance: 'Error creating report instance (Report = {0})',
		errorCreatingReportDocument: 'Error creating report document (Report = {0}; Format = {1})',
		unableToGetReportParameters: "No se puede obtener los parámetros del reporte",
		clientExpired: "Presione el botón 'Recargar' para restaurar la sesión de cliente.",
		menuNavigateBackwardText: 'Navegue hacia atrás',
		menuNavigateBackwardTitle: 'Navegue hacia atrás',
		menuNavigateForwardText: 'Navegue hacia adelante',
		menuNavigateForwardTitle: 'Navegue hacia adelante',
		menuRefreshText: 'Recargar',
		menuRefreshTitle: 'Recargar',
		menuFirstPageText: 'First Page',
		menuFirstPageTitle: 'Primera página',
		menuLastPageText: 'Last Page',
		menuLastPageTitle: 'Ultima página',
		menuPreviousPageTitle: 'Página anterior',
		menuNextPageTitle: 'Siguiente página',
		menuPageNumberTitle: 'Selector de número de página',
		menuDocumentMapTitle: 'Cambiar a mapa del documento',
		menuParametersAreaTitle: 'Cambiar a área de parámetros',
		menuZoomInTitle: 'Acercar',
		menuZoomOutTitle: 'Alejar',
		menuPageStateTitle: 'Cambiar PáginaCompleta/AnchodePágina',
		menuPrintText: 'Imprimir...',
		menuContinuousScrollText: 'Toggle Continuous Scrolling',
		menuSendMailText: 'Send an email',
		menuPrintTitle: 'Imprimir',
		menuContinuousScrollTitle: 'Cambiar a scroll contínuo',
		menuSendMailTitle: 'Enviar un correo',
		menuExportText: 'Export',
		menuExportTitle: 'Exportar',
		menuPrintPreviewText: 'Toggle Print Preview',
		menuPrintPreviewTitle: 'Cambiar a previsualización de impresión',
		menuSearchText: 'Search',
		menuSearchTitle: 'Cambiar a Buscar',
		menuSideMenuTitle: 'Cambiar a menú lateral',
		searchDialogTitle: 'Buscar en el reporte',
		searchDialogSearchInProgress: "Buscando...",
		searchDialogNoResultsLabel: "No se encontraron resultados",
		searchDialogResultsFormatLabel: "Resultado {0} de {1}",
		searchDialogStopTitle: 'Parar búsqueda',
		searchDialogNavigateUpTitle: 'Navegue hacia arriba',
		searchDialogNavigateDownTitle: 'Navegue hacia abajo',
		searchDialogMatchCaseTitle: 'Distingue mayúsculas y minúsculas',
		searchDialogMatchWholeWordTitle: 'Compare la palabra completa',
		searchDialogUseRegexTitle: 'Use Regex',
		searchDialogCaptionText: 'Buscar',
	};

	trv.sr = $.extend(trv.sr, sr);

}(window.telerikReportViewer = window.telerikReportViewer || {}, jQuery));