const connection = new signalR.HubConnectionBuilder()
	.withUrl("/chatHub")
	.build();

connection.on("ReceiveMessage", (senderID, senderAvatar, message) => {
	$('<li class="replies"><img src="' + senderAvatar + '" alt="" /><p>' + message + '</p></li>').appendTo($('.messages ul'));
	$('.message-input input').val(null);
	$('.contact.active .preview').html('<span>You: </span>' + message);
	$(".messages").animate({ scrollTop: $(document).height() }, "fast");

	const friendOnPanel = document.getElementById("friend-panel-" + senderID).firstElementChild.lastElementChild;
	friendOnPanel.lastElementChild.innerHTML = message;
});

connection.on("ShowSentMessage", (receiverID, senderAvatar, message) => {
	$('<li class="sent"><img src="' + senderAvatar + '" alt="" /><p>' + message + '</p></li>').appendTo($('.messages ul'));
	$('.message-input input').val(null);
	$('.contact.active .preview').html('<span>You: </span>' + message);
	$(".messages").animate({ scrollTop: $(document).height() }, "fast");

	const friendOnPanel = document.getElementById("friend-panel-" + receiverID).firstElementChild.lastElementChild;
	friendOnPanel.lastElementChild.innerHTML = message;
});

function SetUserConnected(userID) {
	const friendOnPanelStatus = document.getElementById("friend-panel-" + userID)
	if (friendOnPanelStatus) {
		const status = friendOnPanelStatus.firstElementChild.firstElementChild;
		status.setAttribute("class", "contact-status online");
	}
}

function SetUserDisConnected(userID) {
	const friendOnPanelStatus = document.getElementById("friend-panel-" + userID)
	if (friendOnPanelStatus) {
		const status = friendOnPanelStatus.firstElementChild.firstElementChild;
		status.setAttribute("class", "contact-status offline");
	}
}

connection.on("UserConnected", (userId) => SetUserConnected(userId));

// When user disconnects all others should see his status change
connection.on("UserDisConnected", (userId) => SetUserDisConnected(userId));

// When user connects first time they have to see who is already connected
connection.on("UpdateConnections", (userIDs) => {
	for (const connectedId in userIDs) {
		SetUserConnected(userIDs[connectedId]);
	}
});

async function SendNewMessage() {
	const senderAvatar = document.getElementById("profile-img").getAttribute("src");
	const receiverID = document.getElementById("activeUser").title;
	const senderID = document.getElementById("userName").title;
	const message = document.getElementById("messageInput").value;

	if ($.trim(message) == '') {
		return false;
	}

	connection.invoke("SendMessage", senderAvatar, senderID, receiverID, message).catch(err => console.error(err.toString()));
	sendRequest(receiverID, message);
	event.preventDefault();
}

document.addEventListener('keydown', (e) => {
	if (e.which == 13) {
		SendNewMessage();
		return false;
	}
});

connection.start().catch(err => console.error(err.toString()));