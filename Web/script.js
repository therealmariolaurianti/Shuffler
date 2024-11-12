document.addEventListener('DOMContentLoaded', function () {
  const repoUrl = "https://api.github.com/repos/therealmariolaurianti/Shuffler"; // GitHub API endpoint for the repository
  const commitsUrl = "https://api.github.com/repos/therealmariolaurianti/Shuffler/commits"; // API endpoint for commits
  const repoDescription = document.getElementById('repo-description');
  const repoStars = document.getElementById('repo-stars');
  const repoForks = document.getElementById('repo-forks');
  const repoCommits = document.getElementById('repo-commits');
  const repoLastCommit = document.getElementById('repo-last-commit');
  const repoLastCommitDetails = document.getElementById('repo-last-commit-details');
  const repoAvatar = document.getElementById('repo-avatar');
  const repoLink = document.getElementById('repo-link');

  // Fetch data from GitHub API for the repository
  fetch(repoUrl)
      .then(response => response.json())
      .then(data => {
          // Populate the GitHub preview with data from the repository
          repoDescription.textContent = data.description || "No description available.";
          repoStars.textContent = data.stargazers_count;
          repoForks.textContent = data.forks_count;
          repoAvatar.src = data.owner.avatar_url;
          repoAvatar.alt = data.owner.login + ' Avatar';
          repoLink.href = data.html_url; // Dynamically set the GitHub URL link
      })
      .catch(error => {
          console.error('Error fetching GitHub repo data:', error);
          // Fallback values in case of an error
          repoDescription.textContent = "Unable to load repository details.";
          repoStars.textContent = "N/A";
          repoForks.textContent = "N/A";
      });

  // Function to fetch and calculate the total number of commits (handling pagination)
  function getTotalCommits() {
      let totalCommits = 0;
      let page = 1;
      const commitsPerPage = 30; // Default GitHub API returns 30 commits per page

      function fetchCommits() {
          // Fetch commits for the current page
          fetch(`${commitsUrl}?page=${page}&per_page=${commitsPerPage}`)
              .then(response => response.json())
              .then(data => {
                  totalCommits += data.length; // Add the number of commits from the current page

                  // Check if there are more pages
                  if (data.length === commitsPerPage) {
                      page++; // Increment to get the next page
                      fetchCommits(); // Recursively fetch next page of commits
                  } else {
                      // Once all pages are fetched, display the total commit count
                      repoCommits.textContent = totalCommits;
                  }
              })
              .catch(error => {
                  console.error('Error fetching commits:', error);
                  repoCommits.textContent = "N/A";
              });
      }

      // Start fetching commits from page 1
      fetchCommits();
  }

  // Fetch commit data from the GitHub API to get the last commit message, author, date, and time
  fetch(commitsUrl)
      .then(response => response.json())
      .then(data => {
          // Get the most recent commit
          const lastCommit = data[0];
          if (lastCommit) {
              const commitMessage = lastCommit.commit.message || "No commit message available.";
              const commitAuthor = lastCommit.commit.author.name || "Unknown Author";
              const commitDate = new Date(lastCommit.commit.author.date); // Convert to Date object
              const formattedDate = commitDate.toLocaleString(); // Format date and time in local format

              // Format the commit information
              repoLastCommit.innerHTML = `
                  <strong>Last Commit:</strong><br />
                  <span class="commit-message">${commitMessage}</span>
              `;
              repoLastCommitDetails.innerHTML = `
                  <span class="commit-author">By: <strong>${commitAuthor}</strong></span><br />
                  <span class="commit-date">On: <em>${formattedDate}</em></span>
              `;
          }
      })
      .catch(error => {
          console.error('Error fetching commit data:', error);
          // Fallback values in case of an error
          repoLastCommit.textContent = "N/A";
          repoLastCommitDetails.textContent = "N/A";
      });

  // Call function to get the total number of commits
  getTotalCommits();
});
