const connection = new signalR.HubConnectionBuilder()
	.withUrl("/chatHub")
	.build();

connection.on("ReceiveMessage", (sender, message) => {
	const encodedMsg = sender + " says " + message;
	const li = document.createElement("li");
	li.textContent = encodedMsg;
	document.getElementById("messagesList").appendChild(li);
});

document.getElementById("sendButton").addEventListener("click", event => {
	const sender = document.getElementById("userName").value;
	const receiver = document.getElementById("activeUser").value;
	const message = document.getElementById("messageInput").value;

	connection.invoke("SendMessage", sender, receiver, message).catch(err => console.error(err.toString()));
	sendRequest(receiver, message);
	event.preventDefault();
});

connection.start().catch(err => console.error(err.toString()));