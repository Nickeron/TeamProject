const connection = new signalR.HubConnectionBuilder()
	.withUrl("/chatHub")
	.build();

connection.on("AddTheNewComment", (usersAvatar, Name, AuthorID, isOP, PostID, commentID, commentsText, url) => {
	const commentElement = document.getElementById("listOfComments-" + PostID);

	if (typeof commentElement != 'undefined') {

		const commentInList = document.createElement("li");
		commentInList.setAttribute("id", "listItemComment-" + PostID);

		const divUserAvatar = document.createElement("div");
		divUserAvatar.setAttribute("class", "comment-avatar");

		const userAvatar = document.createElement("img");
		userAvatar.setAttribute("src", usersAvatar);

		divUserAvatar.appendChild(userAvatar);

		commentInList.appendChild(divUserAvatar);

		const commentBox = document.createElement("div");
		commentBox.setAttribute("class", "comment-box");

		const commentHead = document.createElement("div");
		commentHead.setAttribute("class", "comment-head");

		const commentName = document.createElement("h6");
		commentName.setAttribute("class", (isOP) ? "comment-name by-author" : "comment-name");

		const userLink = document.createElement("a");
		userLink.setAttribute("href", "https://" + window.location.host + url);
		userLink.textContent = Name;

		commentName.appendChild(userLink);

		const time = document.createElement("span");
		time.setAttribute("datetime", Date.now());
		time.innerHTML = moment(Date.now()).fromNow();

		commentHead.appendChild(commentName);
		commentHead.appendChild(time);

		if (AuthorID == currentUserID) {
			const close = document.createElement("i");
			close.setAttribute("class", "fas fa-times-circle");
			close.addEventListener("click", function () {
				deleteModal("comment", commentID);
			});
			const edit = document.createElement("i");
			edit.setAttribute("class", "fas fa-pen-square");
			edit.addEventListener("click", function () {
				editModal(commentID, commentsText);
			});

			commentHead.appendChild(close);
			commentHead.appendChild(edit);
		}

		commentBox.appendChild(commentHead);

		const commentText = document.createElement("div");
		commentText.setAttribute("class", "comment-content");
		commentText.innerHTML = commentsText;

		commentBox.appendChild(commentText);

		commentInList.appendChild(commentBox);

		commentElement.insertBefore(commentInList, commentElement.childNodes[0]);
	}
});

async function CreateNewComment(usersAvatar, Name, PostID, url, isOP) {
	const commentsText = document.getElementById("commentText-" + PostID).value;
	const commentID = await SendNewCommentData(PostID);
	console.log(commentID);
	connection.invoke("DistributeComment", usersAvatar, Name, currentUserID, isOP, PostID, commentID, commentsText, url).catch(err => console.error(err.toString()));


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