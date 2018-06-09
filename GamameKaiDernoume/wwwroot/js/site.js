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

moment.locale('el');

// Formats the dates of posts and comments to show as relative
window.setInterval(function () {
    var dateTimeElements = document.getElementsByClassName("post-date");
    for (var i = 0; i < dateTimeElements.length; i++) { formatDate(dateTimeElements[i]); }
}, 5000);

window.setInterval(function () {
    var dateTimeElements = document.getElementsByClassName("message-date");
    for (var i = 0; i < dateTimeElements.length; i++) { formatMessageDate(dateTimeElements[i]); }
}, 2000);

function formatDate(element) {
    element.innerHTML = moment(element.id).add(3, 'hours').fromNow();
}

function formatMessageDate(element) {
    element.innerHTML = moment(element.title).add(3, 'hours').fromNow();
}