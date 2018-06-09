const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub")
    .build();

// Runs on RECEIVER in chat and handles the intro of a new message
connection.on("ReceiveMessage", (senderID, senderAvatar, message, messageID, messageDate) => {
    var audio = new Audio('/sounds/notification.mp3');
    audio.play();

    const activeUserId = document.getElementById("activeUser").title;
    const friendOnPanel = document.getElementById("friend-panel-" + senderID).firstElementChild.lastElementChild;
    const unreadSidePanelElement = document.getElementById("unreadCount-" + senderID);

    // Adds the new message text in the side panel to preview
    friendOnPanel.lastElementChild.innerHTML = message;

    if (unreadSidePanelElement) {
        // Increases the red count of unread messages on the sidepanel
        const oldCount = unreadSidePanelElement.innerHTML;
        const newCount = parseInt(oldCount) + 1;
        unreadSidePanelElement.innerHTML = newCount;

        const sidePanelPreview = document.getElementById("preview-" + senderID);
        sidePanelPreview.appendChild(unreadSidePanelElement);
    }
    else {
        // Creates a new red notification on the sidepanel
        const newUnreadSidePanelElement = document.createElement("strong");
        newUnreadSidePanelElement.setAttribute("id", "unreadCount-" + senderID);
        newUnreadSidePanelElement.setAttribute("class", "badge badge-pill badge-danger float-right");
        newUnreadSidePanelElement.innerHTML = 1;

        const sidePanelPreview = document.getElementById("preview-" + senderID);
        sidePanelPreview.appendChild(newUnreadSidePanelElement);
    }
	/*
	 IF this user is CURRENTLY talking with the sender
													*/
    if (senderID === activeUserId) {
        const unreadMessagesElement = document.getElementById("unreadLatestCount");
        if (unreadMessagesElement) {
            // Increases the red count of unread messages on the head of conversation
            const oldCount = unreadMessagesElement.innerHTML;
            const newCount = parseInt(oldCount) + 1;
            unreadMessagesElement.innerHTML = newCount;
        }
        else {
            // Creates a new red notification on the head of conversation
            const newUnreadMessagesElement = document.createElement("span");
            newUnreadMessagesElement.setAttribute("id", "unreadLatestCount");
            newUnreadMessagesElement.setAttribute("class", "badge badge-pill badge-danger");
            newUnreadMessagesElement.innerHTML = 1;

            const headerMeta = document.getElementById("activeUserMeta")
            headerMeta.appendChild(newUnreadMessagesElement);
        }
        // Adds the new message text in the conversation and scrolls to the bottom
        $('<li class="replies"><img src="' + senderAvatar + '"/><p data-toggle="collapse" data-target="#' + messageID + '" aria-expanded="false"> ' + message + '</p><span class="message-date" id="' + messageID + '" title="' + messageDate + '"></span></li>').appendTo($('.messages ul'));
        $(".messages").scrollTop(1000000);
    }
});

// Runs on SENDER in chat and handles the intro of a new message in conversation and side panel
connection.on("ShowSentMessage", (receiverID, senderAvatar, message, messageID, messageDate) => {
    $('<li class="sent"><img src="' + senderAvatar + '"/><p data-toggle="collapse" data-target="#@message.MessageID" aria-expanded="false"> ' + message + '</p><span class="message-date" id="' + messageID + '" title="' + messageDate + '"></span></li > ').appendTo($('.messages ul'));
    $('.message-input input').val(null);
    $(".messages").scrollTop(1000000);

    const friendOnPanel = document.getElementById("friend-panel-" + receiverID).firstElementChild.lastElementChild;
    friendOnPanel.lastElementChild.innerHTML = message;
});

// When a user connects their status circle gets green
function SetUserConnected(userID) {
    const friendOnPanelStatus = document.getElementById("friend-panel-" + userID)
    if (friendOnPanelStatus) {
        const status = friendOnPanelStatus.firstElementChild.firstElementChild;
        status.setAttribute("class", "contact-status online");
    }
}

// When a user disconnects their status circle gets gray
function SetUserDisConnected(userID) {
    const friendOnPanelStatus = document.getElementById("friend-panel-" + userID)
    if (friendOnPanelStatus) {
        const status = friendOnPanelStatus.firstElementChild.firstElementChild;
        status.setAttribute("class", "contact-status offline");
    }
}

connection.on("UserConnected", (userId) => SetUserConnected(userId));

connection.on("UserDisConnected", (userId) => SetUserDisConnected(userId));

// When user connects first time they have to see who is already connected
connection.on("UpdateConnections", (userIDs) => {
    for (const connectedId in userIDs) {
        SetUserConnected(userIDs[connectedId]);
    }
});

// Handles SignalR distribution of new message and calls sendRequest that saves to database
async function SendNewMessage() {
    const senderAvatar = document.getElementById("profile-img").getAttribute("src");
    const receiverID = document.getElementById("activeUser").title;
    const senderID = document.getElementById("userName").title;
    const message = document.getElementById("messageInput").value;

    if ($.trim(message) == '') { return false; }
    const messageData = await sendRequest(receiverID, message);
    console.log(messageData);
    connection.invoke("SendMessage", senderAvatar, senderID, receiverID, message, messageData.messageID, messageData.messageDate)
        .catch(err => console.error(err.toString()));
    
    event.preventDefault();
}

// When user presses enter and there is text in input
document.addEventListener('keydown', (e) => {
    if (e.which == 13) {
        SendNewMessage();
        return false;
    }
});

connection.start().catch(err => console.error(err.toString()));