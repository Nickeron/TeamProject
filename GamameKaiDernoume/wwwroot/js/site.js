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