document.getElementById("downloadBtn").addEventListener("click", function () {
    const fileUrl =
        "https://github.com/therealmariolaurianti/Shuffler/releases/download/v1.2.0/shufflersetup.exe";
    
    const anchor = document.createElement("a");
    anchor.href = fileUrl;
    anchor.download = ""; // Optional: You can specify a filename here (e.g., 'image.jpg')
  
    document.body.appendChild(anchor); // Add it to the document
    anchor.click(); // Trigger the download
    document.body.removeChild(anchor); // Clean up
});
