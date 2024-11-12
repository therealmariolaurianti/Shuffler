// JavaScript function to handle the download
document.getElementById("downloadBtn").addEventListener("click", function () {
  // The link to the file you want to download
  const fileUrl =
    "https://github.com/therealmariolaurianti/Shuffler/releases/download/Release/shufflersetup.exe"; // Replace with your file's URL

  // Create an invisible anchor tag
  const anchor = document.createElement("a");
  anchor.href = fileUrl;
  anchor.download = ""; // Optional: You can specify a filename here (e.g., 'image.jpg')

  // Programmatically click the anchor tag to trigger the download
  document.body.appendChild(anchor); // Add it to the document
  anchor.click(); // Trigger the download
  document.body.removeChild(anchor); // Clean up
});
