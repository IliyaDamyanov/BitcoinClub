// Landing page scripts — Bitcoin Club Sofia

// Navbar scroll effect
const nav = document.getElementById('mainNav');
window.addEventListener('scroll', () => {
    nav.classList.toggle('scrolled', window.scrollY > 50);
});
