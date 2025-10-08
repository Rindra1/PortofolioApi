// Gestion centralisée du thème
window.siteInterop = {
    hideLoader: function() {
        const loader = document.getElementById('loader');
        if(loader) loader.style.display = 'none';
    },

    initAll: function () {
        console.log("Initialisation du site...");

        this.initMobileMenu();
        this.initSmoothScroll();
        this.initFadeScroll();
        this.initParallax();
        this.initTiltCards();
        this.initModalProjects();
        this.initChatBot();
        this.initTheme();

        // Masquer le loader après tout
        this.hideLoader();
    },

    // --- Thème ---
    initTheme: function () {
        const themeToggle = document.getElementById("theme-toggle");
        const savedTheme = localStorage.getItem("theme") || "dark";

        this.applyTheme(savedTheme);

        if (themeToggle) {
            themeToggle.addEventListener("click", () => {
                const current = localStorage.getItem("theme") || "dark";
                const next = current === "dark" ? "light" : "dark";
                localStorage.setItem("theme", next);
                this.applyTheme(next);
            });
        }

        // Maintenir le thème après navigation ou ancre
        window.addEventListener("popstate", () => this.applyTheme(localStorage.getItem("theme")));
        document.addEventListener("click", e => {
            if (e.target.tagName === "A" && e.target.getAttribute("href")?.startsWith("#")) {
                setTimeout(() => this.applyTheme(localStorage.getItem("theme")), 50);
            }
        });
    },

    applyTheme: function(theme) {
        document.documentElement.setAttribute("data-theme", theme);
        document.body.classList.remove("dark", "light");
        document.body.classList.add(theme);

        const btn = document.getElementById("theme-toggle");
        if (btn) btn.textContent = theme === "dark" ? "☀️" : "🌙";
    },

    // --- ChatBot ---
    initChatBot: function() {
        const toggle = document.getElementById("chat-toggle");
        const chatBody = document.getElementById("chat-body");

        if (!toggle || !chatBody) return;

        toggle.addEventListener("click", () => {
            chatBody.style.display = chatBody.style.display === "flex" ? "none" : "flex";
        });

        window.scrollChatToBottom = () => {
            const chat = document.getElementById('chat-messages');
            if (chat) chat.scrollTop = chat.scrollHeight;
        };
    },

    // --- Menu mobile ---
    initMobileMenu: function () {
        const menuToggle = document.getElementById('menu-toggle');
        const menu = document.getElementById('menu');
        if (!menuToggle || !menu) return;
        menuToggle.addEventListener('click', () => menu.classList.toggle('show'));
    },

    initSmoothScroll: function () {
        document.querySelectorAll('a[href^="#"]').forEach(a => {
            a.addEventListener('click', e => {
                const href = a.getAttribute('href');
                if (href && href.startsWith('#')) {
                    e.preventDefault();
                    const target = document.querySelector(href);
                    if (target) target.scrollIntoView({ behavior: 'smooth' });
                }
            });
        });
    },

    initFadeScroll: function () {
        document.querySelectorAll('.fade').forEach(el => {
            const observer = new IntersectionObserver(entries => {
                entries.forEach(entry => {
                    if (entry.isIntersecting) {
                        entry.target.style.opacity = 1;
                        entry.target.style.transform = 'translateY(0)';
                        observer.unobserve(entry.target);
                    }
                });
            }, { threshold: 0.1 });
            observer.observe(el);
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
                const rotateX = ((y - rect.height / 2) / (rect.height / 2)) * 10;
                const rotateY = ((x - rect.width / 2) / (rect.width / 2)) * 10;
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
        document.addEventListener('click', function(e) {
            const btn = e.target.closest('.open-modal');
            if (btn) {
                e.preventDefault();
                const modal = document.getElementById('modal-' + btn.dataset.project);
                if (modal) modal.style.display = "flex";
            }

            const closeBtn = e.target.closest('.close');
            if (closeBtn) {
                const modal = closeBtn.closest('.modal');
                if (modal) modal.style.display = "none";
            }
        });

        window.addEventListener('click', e => {
            if (e.target.classList.contains('modal')) {
                e.target.style.display = "none";
            }
        });
    }
};

// ⚡ Initialisation automatique après que le DOM est chargé
window.addEventListener("DOMContentLoaded", () => {
    window.siteInterop.initAll();
});
