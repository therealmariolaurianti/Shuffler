// Define constants for the GitHub API and repo data
const username = "therealmariolaurianti"; // Example GitHub username
const repoName = "Shuffler"; // Example GitHub repository name
const commitsPerPage = 30; // Number of commits to fetch per page
let totalCommits = 0; // Total commits fetched
let page = 1; // Page number for pagination

// GitHub API URLs
const repoUrl = `https://api.github.com/repos/${username}/${repoName}`;
const commitsUrl = `https://api.github.com/repos/${username}/${repoName}/commits`;

// DOM elements to display data
const repoDescription = document.getElementById("repo-description");
const repoStars = document.getElementById("repo-stars");
const repoForks = document.getElementById("repo-forks");
const repoAvatar = document.getElementById("repo-avatar");
const repoLink = document.getElementById("repo-link");
const repoCommits = document.getElementById("repo-commits");
const repoLastCommit = document.getElementById("repo-last-commit");
const repoLastCommitDetails = document.getElementById("repo-last-commit-details");

// Fetch and display repo data
function fetchRepoData() {
    fetch(repoUrl)
        .then(response => {
            if (!response.ok) {
                console.error('Failed to fetch repo data', response.status);
                repoDescription.textContent = "Error loading data.";
                return;
            }
            return response.json();
        })
        .then(data => {
            if (data) {
                // Handle missing or empty data fields with fallbacks
                repoDescription.textContent = data.description || "No description available.";
                repoStars.textContent = data.stargazers_count || "N/A";
                repoForks.textContent = data.forks_count || "N/A";
                repoAvatar.src = data.owner.avatar_url || 'default-avatar.jpg'; // Fallback image
                repoLink.href = data.html_url || "#"; // Fallback to a default link
                fetchCommits(); // Fetch commit data once the repo data is loaded
            }
        })
        .catch(error => {
            console.error('Error fetching repo data:', error);
            repoDescription.textContent = "Error loading data.";
        });
}

// Fetch and count commits using pagination
function fetchCommits() {
    fetch(`${commitsUrl}?page=${page}&per_page=${commitsPerPage}`)
        .then(response => response.json())
        .then(data => {
            if (data.length > 0) {
                totalCommits += data.length; // Add the number of commits on this page
                if (data.length === commitsPerPage) {
                    // Continue fetching if there are more commits
                    page++;
                    fetchCommits();
                } else {
                    // No more commits, finalize the total commit count
                    repoCommits.textContent = totalCommits;
                }
            } else {
                // No commits found
                repoCommits.textContent = "0";
            }
        })
        .catch(error => {
            console.error('Error fetching commits:', error);
            repoCommits.textContent = "N/A"; // Fallback if there's an error
        });
}

// Fetch and display the last commit's details
function fetchLastCommit() {
    fetch(commitsUrl + '?page=1&per_page=1') // Only get the first commit (latest)
        .then(response => response.json())
        .then(data => {
            if (data.length > 0) {
                const lastCommit = data[0];
                repoLastCommit.textContent = lastCommit.committer.date ? `Last commit: ${lastCommit.committer.date}` : "No commit date available.";
                repoLastCommitDetails.textContent = lastCommit.commit.message || "No commit message available.";
            } else {
                repoLastCommit.textContent = "No commits yet.";
                repoLastCommitDetails.textContent = "N/A";
            }
        })
        .catch(error => {
            console.error('Error fetching last commit:', error);
            repoLastCommit.textContent = "Error loading commit data.";
            repoLastCommitDetails.textContent = "N/A";
        });
}

// Check for API rate limit
function checkRateLimit() {
    fetch('https://api.github.com/rate_limit')
        .then(response => response.json())
        .then(data => {
            const remaining = data.resources.core.remaining;
            if (remaining === 0) {
                console.warn("API rate limit reached. Please try again later.");
            }
        })
        .catch(error => console.error('Error checking rate limit:', error));
}

// Initialize the page
function init() {
    checkRateLimit(); // Check the rate limit before making requests
    fetchRepoData(); // Fetch and display the repository data
    fetchLastCommit(); // Fetch the last commit details
}

// Run the initialization function once the DOM is loaded
document.addEventListener('DOMContentLoaded', init);
