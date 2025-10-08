window.themeManager = {
    // Appliquer le thème sauvegardé
    applyTheme: function () {
        const theme = localStorage.getItem("theme") || "dark";
        document.documentElement.setAttribute("data-theme", theme);
        document.body.classList.remove("dark", "light");
        document.body.classList.add(theme);

        // Change l’icône du bouton
        const btn = document.getElementById("theme-toggle");
        if (btn) btn.textContent = theme === "dark" ? "☀️" : "🌙";
    },

    // Changer de thème
    toggleTheme: function () {
        const current = localStorage.getItem("theme") || "dark";
        const next = current === "dark" ? "light" : "dark";
        localStorage.setItem("theme", next);
        this.applyTheme();
        return next;
    },

    // Initialisation globale
    init: function () {
        // Appliquer le thème au chargement
        this.applyTheme();

        // Gérer le bouton toggle
        const btn = document.getElementById("theme-toggle");
        if (btn) {
            btn.addEventListener("click", () => {
                this.toggleTheme();
            });
        }

        // S'assurer qu'après navigation (Blazor ou <a>) le thème reste
        window.addEventListener("popstate", () => this.applyTheme());
        document.addEventListener("click", e => {
            if (e.target.tagName === "A" && e.target.getAttribute("href")?.startsWith("#")) {
                setTimeout(() => this.applyTheme(), 50);
            }
        });
    }
};

// Exécution automatique après chargement de la page
window.addEventListener("DOMContentLoaded", () => {
    window.themeManager.init();
});




// script.js
window.siteInterop = {
    initAll: function () {
        console.log("Initialisation du site...");

        this.initMobileMenu();
        this.initSmoothScroll();
        this.initFadeScroll();
        this.initParallax();
        this.initTiltCards();
        this.initModalProjects();
        this.IniChatBot();
        this.initTheme(); // ✅ seul appel correct ici
    },

    // --- 🎨 Thème : gestion centralisée ---
    initTheme: function () {
        const themeToggle = document.getElementById("theme-toggle");
        const savedTheme = localStorage.getItem("theme") || "dark";

        // Appliquer le thème sauvegardé
        this.applyTheme(savedTheme);

        // Gérer le clic sur le bouton
        if (themeToggle) {
            themeToggle.addEventListener("click", () => {
                const current = localStorage.getItem("theme") || "dark";
                const next = current === "dark" ? "light" : "dark";
                localStorage.setItem("theme", next);
                this.applyTheme(next);
            });
        }

        // Réappliquer après navigation (ancre ou Blazor)
        window.addEventListener("popstate", () => this.applyTheme(localStorage.getItem("theme")));
        document.addEventListener("click", e => {
            if (e.target.tagName === "A" && e.target.getAttribute("href")?.startsWith("#")) {
                setTimeout(() => this.applyTheme(localStorage.getItem("theme")), 50);
            }
        });
    },

    applyTheme: function (theme) {
        document.documentElement.setAttribute("data-theme", theme);
        document.body.classList.remove("dark", "light");
        document.body.classList.add(theme);

        const btn = document.getElementById("theme-toggle");
        if (btn) btn.textContent = theme === "dark" ? "☀️" : "🌙";
    },

    // --- 🤖 Chatbot ---
    IniChatBot: function () {
        const toggle = document.getElementById("chat-toggle");
        const chatBody = document.getElementById("chat-body");

        if (!toggle || !chatBody) {
            console.warn("Chatbot non trouvé dans le DOM");
            return;
        }

        toggle.addEventListener("click", () => {
            chatBody.style.display = chatBody.style.display === "flex" ? "none" : "flex";
        });

        window.scrollChatToBottom = () => {
            const chat = document.getElementById('chat-messages');
            if (chat) chat.scrollTop = chat.scrollHeight;
        };
    },

    // --- Autres fonctions inchangées ---
    initMobileMenu: function () {
        const menuToggle = document.getElementById('menu-toggle');
        const menu = document.getElementById('menu');
        if (!menuToggle || !menu) return;
        menuToggle.addEventListener('click', () => menu.classList.toggle('show'));
    },

    /*initSmoothScroll: function () {
        document.querySelectorAll('nav ul li a:not(.btn-hero)').forEach(a => {
            a.addEventListener('click', e => {
                const href = a.getAttribute('href');
                if (href && href.startsWith('#')) {
                    e.preventDefault();
                    const target = document.querySelector(href);
                    if (target) target.scrollIntoView({ behavior: 'smooth' });
                }
            });
        });
    },*/

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
        const fadeElems = document.querySelectorAll('.fade');
        const observer = new IntersectionObserver(entries => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    entry.target.style.opacity = 1;
                    entry.target.style.transform = 'translateY(0)';
                    observer.unobserve(entry.target);
                }
            });
        }, { threshold: 0.1 });
        fadeElems.forEach(el => observer.observe(el));
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
        document.querySelectorAll('.open-modal').forEach(btn => btn.onclick = null);
        document.querySelectorAll('.close').forEach(btn => btn.onclick = null);

        document.addEventListener('click', function (e) {
            const btn = e.target.closest('.open-modal');
            if (!btn) return;
            e.preventDefault();
            const projectId = btn.dataset.project;
            const modal = document.getElementById('modal-' + projectId);
            if (modal) modal.style.display = "flex";
        });

        document.querySelectorAll('.close').forEach(c => {
            c.addEventListener('click', () => {
                const modal = c.closest('.modal');
                if (modal) modal.style.display = "none";
            });
        });

        window.addEventListener('click', e => {
            if (e.target.classList.contains('modal')) {
                e.target.style.display = "none";
            }
        });
    }
};






