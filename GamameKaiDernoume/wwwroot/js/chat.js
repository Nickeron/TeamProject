const connection = new signalR.HubConnectionBuilder()
	.withUrl("/chatHub")
	.build();

connection.on("ReceiveMessage", (senderID, senderAvatar, message) => {
	const activeUserId = document.getElementById("activeUser").title;
	const friendOnPanel = document.getElementById("friend-panel-" + senderID).firstElementChild.lastElementChild;
	const unreadSidePanelElement = document.getElementById("unreadCount-" + senderID);
	// Adds the new message in the side panel to preview
	friendOnPanel.lastElementChild.innerHTML = message;

	if (unreadSidePanelElement) {
		const oldCount = unreadSidePanelElement.innerHTML;
		const newCount = parseInt(oldCount) + 1;
		unreadSidePanelElement.innerHTML = newCount;

		const sidePanelPreview = document.getElementById("preview-" + senderID);
		sidePanelPreview.appendChild(unreadSidePanelElement);
	}
	else {
		const newUnreadSidePanelElement = document.createElement("strong");
		newUnreadSidePanelElement.setAttribute("id", "unreadCount-" + senderID);
		newUnreadSidePanelElement.setAttribute("class", "badge badge-pill badge-danger float-right");
		newUnreadSidePanelElement.innerHTML = 1;

		const sidePanelPreview = document.getElementById("preview-" + senderID);
		sidePanelPreview.appendChild(newUnreadSidePanelElement);
	}

	if (senderID === activeUserId) {
		const unreadMessagesElement = document.getElementById("unreadLatestCount");
		if (unreadMessagesElement) {
			// Increases the red count of unread messages on the head of conversation
			const oldCount = unreadMessagesElement.innerHTML;
			const newCount = parseInt(oldCount) + 1;
			unreadMessagesElement.innerHTML = newCount;
		}
		else {
			const newUnreadMessagesElement = document.createElement("span");
			newUnreadMessagesElement.setAttribute("id", "unreadLatestCount");
			newUnreadMessagesElement.setAttribute("class", "badge badge-pill badge-danger");
			newUnreadMessagesElement.innerHTML = 1;

			const headerMeta = document.getElementById("activeUserMeta")
			headerMeta.appendChild(newUnreadMessagesElement);
		}
		$('<li class="replies"><img src="' + senderAvatar + '" alt="" /><p title="' + moment().fromNow() + '" > ' + message + '</p></li>').appendTo($('.messages ul'));
		$('.message-input input').val(null);
		$('.contact.active .preview').html('<span>You: </span>' + message);
		$(".messages").scrollTop(1000000);
	}
});

connection.on("ShowSentMessage", (receiverID, senderAvatar, message) => {
	$('<li class="sent"><img src="' + senderAvatar + '" alt="" /><p title="' + moment().fromNow() + '" > ' + message + '</p ></li > ').appendTo($('.messages ul'));
	$('.message-input input').val(null);
	$('.contact.active .preview').html('<span>You: </span>' + message);
	$(".messages").scrollTop(1000000);

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