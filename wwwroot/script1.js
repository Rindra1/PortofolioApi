// script.js
window.siteInterop = {
    initAll: function () {
        console.log("Initialisation du site...");

        this.initMobileMenu();
        this.initSmoothScroll();
        this.initFadeScroll();
        this.initThemeToggle();
        this.initParallax();
        this.initTiltCards();
        this.initModalProjects();
    },

    initMobileMenu: function () {
        const menuToggle = document.getElementById('menu-toggle');
        const menu = document.getElementById('menu');
        if (!menuToggle || !menu) return;

        menuToggle.addEventListener('click', () => menu.classList.toggle('show'));
    },

    initSmoothScroll: function () {
        document.querySelectorAll('nav ul li a').forEach(a => {
            a.addEventListener('click', e => {
                e.preventDefault();
                const target = document.querySelector(a.getAttribute('href'));
                if (target) target.scrollIntoView({ behavior: 'smooth' });
            });
        });
    },

    initFadeScroll: function () {
        const fadeElems = document.querySelectorAll('.fade');
        const observer = new IntersectionObserver(entries => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    entry.target.style.opacity = 1;
                    entry.target.style.transform = 'translateY(0)';
                    observer.unobserve(entry.target);
                }
            });
        }, { threshold: 0.2 });
        fadeElems.forEach(el => observer.observe(el));
    },

    initThemeToggle: function () {
        const toggleBtn = document.getElementById('theme-toggle');
        if (!toggleBtn) return;

        toggleBtn.addEventListener('click', () => {
            document.body.classList.toggle('dark');
            document.body.classList.toggle('light');
            toggleBtn.textContent = document.body.classList.contains('dark') ? '☀️' : '🌙';
        });
    },

    initParallax: function () {
        window.addEventListener('scroll', () => {
            const y = window.scrollY;
            const back = document.querySelector('.layer-back');
            const mid = document.querySelector('.layer-mid');
            const front = document.querySelector('.layer-front');
            if (back) back.style.transform = `translateY(${y * 0.2}px)`;
            if (mid) mid.style.transform = `translateY(${y * 0.5}px)`;
            if (front) front.style.transform = `translateY(${y * 0.8}px)`;
        });
    },

    initTiltCards: function () {
        document.querySelectorAll('.tilt').forEach(card => {
            card.addEventListener('mousemove', e => {
                const rect = card.getBoundingClientRect();
                const x = e.clientX - rect.left, y = e.clientY - rect.top;
                const centerX = rect.width / 2, centerY = rect.height / 2;
                const rotateX = ((y - centerY) / centerY) * 10;
                const rotateY = ((x - centerX) / centerX) * 10;
                const img = card.querySelector('img');
                if (img) img.style.transform = `rotateX(${-rotateX}deg) rotateY(${rotateY}deg) scale(1.05)`;
            });
            card.addEventListener('mouseleave', () => {
                const img = card.querySelector('img');
                if (img) img.style.transform = 'rotateX(0) rotateY(0) scale(1)';
            });
        });
    },

    initModalProjects: function () {
        // Supprime les anciens listeners pour éviter doublons
        document.querySelectorAll('.open-modal').forEach(btn => btn.onclick = null);
        document.querySelectorAll('.close').forEach(btn => btn.onclick = null);

        // Ouvrir modal
        document.addEventListener('click', function(e) {
    const btn = e.target.closest('.open-modal');
    if (!btn) return;
    e.preventDefault();
    
    const projectId = btn.dataset.project;
    alert('modal-' + projectId); // ton test
    const modal = document.getElementById('modal-' + projectId);
    if (modal) modal.style.display = "flex";
});

        // Fermer modal
        document.querySelectorAll('.close').forEach(c => {
            c.addEventListener('click', () => {
                const modal = c.closest('.modal');
                if (modal) modal.style.display = "none";
            });
        });

        // Fermer modal en cliquant en dehors
        window.addEventListener('click', e => {
            if (e.target.classList.contains('modal')) {
                e.target.style.display = "none";
            }
        });
    }
};
