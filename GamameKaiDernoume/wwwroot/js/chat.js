const connection = new signalR.HubConnectionBuilder()
	.withUrl("/chatHub")
	.build();

connection.on("ReceiveMessage", (senderAvatar, message) => {
	$('<li class="replies"><img src="' + senderAvatar + '" alt="" /><p>' + message + '</p></li>').appendTo($('.messages ul'));
	$('.message-input input').val(null);
	$('.contact.active .preview').html('<span>You: </span>' + message);
	$(".messages").animate({ scrollTop: $(document).height() }, "fast");
});

connection.on("ShowSentMessage", (senderAvatar, message) => {
	$('<li class="sent"><img src="'+senderAvatar+'" alt="" /><p>' + message + '</p></li>').appendTo($('.messages ul'));
	$('.message-input input').val(null);
	$('.contact.active .preview').html('<span>You: </span>' + message);
	$(".messages").animate({ scrollTop: $(document).height() }, "fast");
});

document.getElementById("sendButton").addEventListener("click", event => {
	const senderAvatar = document.getElementById("profile-img").getAttribute("src");
	const receiver = document.getElementById("activeUser").title;
	const message = document.getElementById("messageInput").value;

	if ($.trim(message) == '') {
		return false;
	}

	connection.invoke("SendMessage", senderAvatar, receiver, message).catch(err => console.error(err.toString()));
	sendRequest(receiver, message);
	event.preventDefault();
});

connection.start().catch(err => console.error(err.toString()));