async function sendData(url, data) {
	const urlToSendRequest = "https://" + window.location.host + url;

	const rawData = await fetch(url, {
		method: 'POST',
		headers: {
			'Accept': 'application/json',
			'Content-Type': 'application/json',
		},
		credentials: "same-origin",
		body: JSON.stringify(data)
	});

	return rawData.json();
}

window.onload = function () {
	var dateTimeElements = document.getElementsByClassName("post-date");

	for (var i = 0; i < dateTimeElements.length; i++) {
		formatDate(dateTimeElements[i]);
	}
};

function formatDate(element) {
	console.log(element.innerHTML);
	element.innerHTML = moment(element.innerHTML).fromNow();
}