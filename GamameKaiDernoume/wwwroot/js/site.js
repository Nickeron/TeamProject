// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

async function sendData(url, data) {
	const urlToSendRequest = "https://" + window.location.host + url;

	const RawResponse = await fetch(url, {
			method: 'POST',
			headers: {
				'Accept': 'application/json',
				'Content-Type': 'application/json',
			},
			credentials: "same-origin",
			body: JSON.stringify(data)
	});

	return await RawResponse.json();
}