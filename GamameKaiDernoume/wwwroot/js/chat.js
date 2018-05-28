const connection = new signalR.HubConnectionBuilder()
	.withUrl("/chatHub")
	.build();

connection.on("ReceiveMessage", (sender, message) => {
	const encodedMsg = sender + " says " + message;
	const li = document.createElement("li");
	li.textContent = encodedMsg;
	document.getElementById("messagesList").appendChild(li);
});

connection.on("ShowSentMessage", (sender, message) => {
	$('<li class="sent"><img src="http://emilcarlsson.se/assets/mikeross.png" alt="" /><p>' + message + '</p></li>').appendTo($('.messages ul'));
	$('.message-input input').val(null);
	$('.contact.active .preview').html('<span>You: </span>' + message);
	$(".messages").animate({ scrollTop: $(document).height() }, "fast");
});

document.getElementById("sendButton").addEventListener("click", event => {
	const sender = document.getElementById("userName").value;
	const receiver = document.getElementById("activeUser").value;
	const message = document.getElementById("messageInput").value;

	if ($.trim(message) == '') {
		return false;
	}

	connection.invoke("SendMessage", sender, receiver, message).catch(err => console.error(err.toString()));
	sendRequest(receiver, message);
	event.preventDefault();
});

connection.start().catch(err => console.error(err.toString()));