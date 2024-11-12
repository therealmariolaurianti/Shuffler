// Get the last modified date of the document
var lastUpdated = document.lastModified;

// Display the last updated date in the designated <span>
document.addEventListener('DOMContentLoaded', function () {
    document.getElementById('lastUpdated').innerText = lastUpdated;
});