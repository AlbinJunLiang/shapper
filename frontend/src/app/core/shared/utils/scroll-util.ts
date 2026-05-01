export function scrollToTop() {
  setTimeout(() => {
    window.scrollTo({
      top: 50,
      behavior: 'smooth'
    });
  }, 100); 
}