$(function () {
	setTogglerIcon();

	// Dropdown menu ¿?
	$(".sidebar-dropdown > a").click(function () {
		var menu = $(this).closest("ul");
		menu.find(".sidebar-submenu").slideUp(200);
		if ($(this).parent().hasClass("active")) {
			menu.find(".sidebar-dropdown").removeClass("active");
			$(this).parent().removeClass("active");
		} else {
			menu.find(".sidebar-dropdown").removeClass("active");
			$(this).next(".sidebar-submenu").slideDown(200);
			$(this).parent().addClass("active");
		}
	});

	//toggle sidebar
	$("#toggle-sidebar").click(function () {
		$(".page-wrapper").toggleClass("toggled");
		setTogglerIcon();
	});

	//Pin sidebar
	$("#pin-sidebar").click(function () {
		if ($(".page-wrapper").hasClass("pinned")) {
			// unpin sidebar when hovered
			$(".page-wrapper").removeClass("pinned");
			$("#sidebar").unbind("hover");
		} else {
			$(".page-wrapper").addClass("pinned");
			$("#sidebar").hover(
				function () {
					console.log("mouseenter");
					$(".page-wrapper").addClass("sidebar-hovered");
				},
				function () {
					console.log("mouseout");
					$(".page-wrapper").removeClass("sidebar-hovered");
				}
			)
		}
	});

	//toggle sidebar overlay
	$("#overlay").click(function () {
		$(".page-wrapper").toggleClass("toggled");
	});

	//switch between themes 
	var themes = "default-theme legacy-theme chiller-theme ice-theme cool-theme light-theme";
	$('[data-theme]').click(function () {
		$('[data-theme]').removeClass("selected");
		$(this).addClass("selected");
		$('.page-wrapper').removeClass(themes);
		$('.page-wrapper').addClass($(this).attr('data-theme'));
	});

	// switch between background images
	var bgs = "bg1 bg2 bg3 bg4";
	$('[data-bg]').click(function () {
		$('[data-bg]').removeClass("selected");
		$(this).addClass("selected");
		$('.page-wrapper').removeClass(bgs);
		$('.page-wrapper').addClass($(this).attr('data-bg'));
	});

	// toggle background image
	$("#toggle-bg").change(function (e) {
		e.preventDefault();
		$('.page-wrapper').toggleClass("sidebar-bg");
	});

	// toggle border radius
	$("#toggle-border-radius").change(function (e) {
		e.preventDefault();
		$('.page-wrapper').toggleClass("boder-radius-on");
	});

	//custom scroll bar is only used on desktop
	if (!/Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent)) {
		$(".sidebar-content").mCustomScrollbar({
			axis: "y",
			autoHideScrollbar: true,
			scrollInertia: 300
		});
		$(".sidebar-content").addClass("desktop");
	}

	//#region Menu Inferior

	//Abre el carrito de compras
	//$("#m-shoppingcart").click(function (e) {
	//	e.preventDefault();
	//	if ($("#UserCardCode").val() === "CDMC-002") {
	//		TempSale();
	//	} else {
	//		ShoppingCart();
	//	}
	//});

	//Abre las notificaciones
	$("#m-notifications").click(function (e) {
		e.preventDefault();
		Notifications(true);
	});

	//COnfirmación de que se quiere cerrar sesión
	$("#m-logout").click(function (e) {
		e.preventDefault();
		showConfirm("¿Está seguro que desea cerrar su sesión?", () => { window.location.href = urlLogOut; }, null, "Si, cerrar sesión");
	});

	//#endregion
});

function setTogglerIcon() {
	if ($(window).width() < 768) {
		$("#toggle-sidebar i").removeClass("fa-arrow-left").addClass("fa-bars");
	} else {
		if ($(".page-wrapper").hasClass("toggled")) {
			$("#toggle-sidebar i").addClass("fa-arrow-left").removeClass("fa-bars");
		} else {
			$("#toggle-sidebar i").removeClass("fa-arrow-left").addClass("fa-bars");
		}
	}
}