function createCookie(name, value, days) {
    let expiration = "";
	if (days) {
		const date = new Date();
		date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
		expiration = "; expires=" + date.toGMTString();
	}

	document.cookie = name + "=" + value + expiration + "; path=/";
}

function readCookie(name) {
	const nameEQ = name + "=";
	const ca = document.cookie.split(';');

	for(var i = 0; i < ca.length; i++) {
		let c = ca[i];
		while (c.charAt(0)==' ') c = c.substring(1, c.length);
		if (c.indexOf(nameEQ) == 0) {
			console.log("RC found:", [c.substring(nameEQ.length, c.length)])
			return c.substring(nameEQ.length, c.length);
		}
	}

	console.log("RC found fuck all.")

	return undefined;
}

function eraseCookie(name) {
	createCookie(name, "", -1);
}