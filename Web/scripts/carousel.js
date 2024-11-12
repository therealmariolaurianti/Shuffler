let currentIndex = 0;
const images = document.querySelectorAll('.carousel-wrapper img');
const totalImages = images.length;
const indicators = document.querySelectorAll('.carousel-indicators button');

// Function to move to the next or previous slide
function moveSlide(step) {
    currentIndex += step;

    if (currentIndex < 0) {
        currentIndex = totalImages - 1;  // Loop back to last image
    } else if (currentIndex >= totalImages) {
        currentIndex = 0;  // Loop back to first image
    }

    updateCarousel();
}

// Function to go to a specific slide based on indicator click
function goToSlide(index) {
    currentIndex = index;
    updateCarousel();
}

// Update the carousel position based on the current index
function updateCarousel() {
    const offset = -currentIndex * 100;  // Move images to the left
    document.querySelector('.carousel-wrapper').style.transform = `translateX(${offset}%)`;

    // Update active indicator
    indicators.forEach((indicator, idx) => {
        if (idx === currentIndex) {
            indicator.classList.add('active');
        } else {
            indicator.classList.remove('active');
        }
    });
}

// Auto slide every 5 seconds
setInterval(() => {
    moveSlide(1);
}, 5000);

// Initialize the carousel on page load
window.addEventListener('load', updateCarousel);