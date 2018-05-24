const connection = new signalR.HubConnectionBuilder()
	.withUrl("/chatHub")
	.build();

connection.on("AddTheNewComment", (Name, PostID, commentsText) => {
	const commentInList = document.createElement("li");
	commentInList.setAttribute("id", "listItemComment-" + PostID);
	commentInList.setAttribute("class", "list-group-item");
	commentInList.textContent = Name + ': ' + commentsText;
	document.getElementById("listOfComments-" + PostID).appendChild(commentInList);
	});
	
function CreateNewComment(PostID) {
	const commentsText = document.getElementById("commentText-" + PostID).value;
	console.log(commentsText, PostID);
	connection.invoke("CreateNewComment", PostID, commentsText).catch(err => console.error(err.toString()));
	SendNewCommentData(PostID);
	event.preventDefault();
};

connection.start().catch(err => console.error(err.toString()));