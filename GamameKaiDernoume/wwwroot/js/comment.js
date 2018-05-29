const connection = new signalR.HubConnectionBuilder()
	.withUrl("/chatHub")
	.build();

connection.on("AddTheNewComment", (Name, usersAvatar, PostID, commentsText) => {
	const commentElement = document.getElementById("listOfComments-" + PostID);
	if (commentElement) {
		const commentInList = document.createElement("li");
		commentInList.setAttribute("id", "listItemComment-" + PostID);
		commentInList.setAttribute("class", "list-group-item");

		const userAvatar = document.createElement("img");
		userAvatar.setAttribute("src", usersAvatar);
		userAvatar.setAttribute("class", "avatar rounded-circle");

		const userLink = document.createElement("a");
		userLink.setAttribute("href", "https://" + window.location.host + url);

		const usersName = document.createElement("span");
		usersName.textContent = Name;

		userLink.appendChild(usersName);

		const commentText = document.createElement("text");
		commentText.textContent = commentsText;

		commentInList.appendChild(userAvatar);
		commentInList.appendChild(usersName);

		commentElement.appendChild(commentInList);
	}
});

async function CreateNewComment(PostID) {
	const commentsText = document.getElementById("commentText-" + PostID).value;
	console.log(commentsText, PostID);
	connection.invoke("DistributeComment", PostID, commentsText).catch(err => console.error(err.toString()));
	await SendNewCommentData(PostID);
	event.preventDefault();
};

connection.on("AddTheReaction", (PostID, likes, dislikes) => {
	document.getElementById("like-count-" + PostID).innerHTML = likes;
	document.getElementById("dislike-count-" + PostID).innerHTML = dislikes;
});

async function AddReaction(PostID, isLike) {
	const result = await SendReaction(PostID, isLike);
	connection.invoke("DistributeReaction", PostID, result.likes, result.dislikes).catch(err => console.error(err.toString()));
};

connection.start().catch(err => console.error(err.toString()));