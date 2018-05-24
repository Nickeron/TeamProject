// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

async function sendData(url, data) {
	const urlToSendRequest = "https://" + window.location.host + url;
	let rawData;
	await fetch(url, {
		method: 'POST',
		headers: {
			'Accept': 'application/json',
			'Content-Type': 'application/json',
		},
		credentials: "same-origin",
		body: JSON.stringify(data)
	})
		.then(res => res.json())
		.catch(error => console.error('Error:', error))
		.then(response => {
			console.log('Success:', response);
			rawData = response;
		});
	return rawData;
}