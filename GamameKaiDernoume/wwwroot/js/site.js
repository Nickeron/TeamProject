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

// Formats the dates of posts and comments to show as relative
window.onload = function () {
	var dateTimeElements = document.getElementsByClassName("post-date");

	for (var i = 0; i < dateTimeElements.length; i++) {
		formatDate(dateTimeElements[i]);
	}
};

function formatDate(element) {
	element.innerHTML = moment(element.id).fromNow();
}