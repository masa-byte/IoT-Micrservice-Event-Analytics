"use strict";

let connection = new signalR.HubConnectionBuilder().withUrl("/eventHub").build();
connection.start();
console.log(connection);

connection.on("ReceiveEvent", function (event) {
    let table = document.getElementById("event-table");
    console.log(event);

    let row = document.createElement("tr");

    let cell1 = document.createElement("td");
    let cell2 = document.createElement("td");
    let cell3 = document.createElement("td");
    let cell4 = document.createElement("td");
    let cell5 = document.createElement("td");

    cell1.textContent = event.id;
    cell2.textContent = event.eventType;
    cell3.textContent = event.value.toFixed(2);
    cell4.textContent = event.eventDate;
    cell5.textContent = event.locationId;

    row.appendChild(cell1);
    row.appendChild(cell2);
    row.appendChild(cell3);
    row.appendChild(cell4);
    row.appendChild(cell5);
    table.appendChild(row);
});

connection.onclose((error) => {
    console.log("Connection closed:", error);
});