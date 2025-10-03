// Menu burger

// script.js
window.siteInterop = {
    initAll: function () {
        console.log("Initialisation du site...");
        window.siteInterop.initMenu();
        window.siteInterop.initModal();
    },

    initMenu: function () {
        const toggle = document.getElementById('toggle');
        const menu = document.getElementById('menu');
        toggle.addEventListener('click', () => menu.classList.toggle('show'));
    },

    initModal: function () {
        const modal = document.getElementById('modal');
        const modalImage = document.getElementById('modal-image');
        const closeModal = document.querySelector('.close');
const prevBtn = document.querySelector('.prev');
const nextBtn = document.querySelector('.next');

let images = [];
let currentIndex = 0;

document.querySelectorAll('.btn-details').forEach(btn => {
  btn.addEventListener('click', (e) => {
    const card = e.target.closest('.project-card');
    images = card.dataset.images.split(',');
    currentIndex = 0;
    showImage(currentIndex);
    modal.style.display = 'flex';
  });
});

function showImage(index) {
  modalImage.style.opacity = 0;
  setTimeout(() => {
    modalImage.src = images[index];
    modalImage.style.opacity = 1;
  }, 300);
}

prevBtn.addEventListener('click', () => {
  currentIndex = (currentIndex - 1 + images.length) % images.length;
  showImage(currentIndex);
   modal.classList.add('show');
});

nextBtn.addEventListener('click', () => {
  currentIndex = (currentIndex + 1) % images.length;
  showImage(currentIndex);
   modal.classList.add('show');
});

closeModal.addEventListener('click', () => {
  modal.style.display = 'none';
  modal.classList.remove('show');
});

window.addEventListener('click', (e) => {
  if (e.target === modal) modal.style.display = 'none';
});




// Sections animation on scroll
const sections = document.querySelectorAll('section');

const observer = new IntersectionObserver(entries => {
  entries.forEach(entry => {
    if(entry.isIntersecting){
      entry.target.style.opacity = 1;
      entry.target.style.transform = 'translateY(0)';
    }
  });
},{ threshold: 0.2 });

sections.forEach(section => observer.observe(section));



let startX = 0;

modalImage.addEventListener('touchstart', e => {
  startX = e.touches[0].clientX;
});

modalImage.addEventListener('touchend', e => {
  let endX = e.changedTouches[0].clientX;
  if (endX - startX > 50) {
    currentIndex = (currentIndex - 1 + images.length) % images.length;
    showImage(currentIndex);
  } else if (startX - endX > 50) {
    currentIndex = (currentIndex + 1) % images.length;
    showImage(currentIndex);
  }
});


/*animation*/
window.addEventListener('scroll', () => {
  const navbar = document.querySelector('.navbar');
  if(window.scrollY > 50){
    navbar.classList.add('scrolled');
  } else {
    navbar.classList.remove('scrolled');
  }
});

    }
};

// Quand Blazor rend du contenu, on relance initAll
window.Blazor.start = (function (orig) {
    return async function () {
        const result = await orig(); // exécute le vrai Blazor.start
        Blazor.addEventListener("afterRender", () => {
            window.siteInterop.initAll();
        });
        return result;
    };
});





