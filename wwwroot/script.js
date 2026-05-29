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

        const saved = localStorage.getItem("theme");
        const systemPref = (window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches) ? 'dark' : 'light';
        const themeToUse = saved || systemPref || 'dark';

        console.log("Thème chargé :", themeToUse);

        this.applyTheme(themeToUse);

        const themeToggle = document.getElementById("theme-toggle");

        if (themeToggle) {
            themeToggle.onclick = null;
            themeToggle.onclick = () => {
                const current = localStorage.getItem("theme") || (document.body.classList.contains('dark') ? 'dark' : 'light');
                const next = current === 'dark' ? 'light' : 'dark';
                localStorage.setItem("theme", next);
                this.applyTheme(next);
                console.log("Nouveau thème :", next);
            };
        }
    },

    applyTheme: function(theme) {

        document.body.classList.remove("dark", "light");

        document.body.classList.add(theme);

        document.documentElement.setAttribute("data-theme", theme);

        const btn = document.getElementById("theme-toggle");

        if (btn) {
            btn.textContent = theme === "dark"
                ? "☀️"
                : "🌙";
        }
    },

    // --- ChatBot ---
    initChatBot: function () {
    const toggle = document.getElementById("chat-toggle");
    const chatBody = document.getElementById("chat-body");

    if (!toggle || !chatBody) {
        console.log("Chatbot introuvable");
        return;
    }

    // Évite doublons d'événements
    toggle.onclick = null;

    // Accessibilité initiale
    toggle.setAttribute('role', 'button');
    toggle.setAttribute('tabindex', '0');
    toggle.setAttribute('aria-controls', 'chat-body');
    toggle.setAttribute('aria-expanded', 'false');
    chatBody.setAttribute('aria-hidden', 'true');

    toggle.onclick = () => {
        const isOpen = chatBody.classList.toggle('show');
        toggle.setAttribute('aria-expanded', isOpen ? 'true' : 'false');
        chatBody.setAttribute('aria-hidden', isOpen ? 'false' : 'true');
        // focus sur le champ input quand on ouvre
        if (isOpen) {
            const input = document.getElementById('chat-input-field');
            if (input) setTimeout(() => input.focus(), 120);
        }
    };

    // Envoi par Entrée
    const input = document.getElementById('chat-input-field');
    const sendBtn = document.getElementById('chat-send-btn');
    if (input) {
        input.addEventListener('keydown', (e) => {
            if (e.key === 'Enter' && !e.shiftKey) {
                e.preventDefault();
                if (sendBtn) sendBtn.click();
            }
        });
    }

    // Fonction globale scroll
    window.scrollChatToBottom = () => {
        const chat = document.getElementById('chat-messages');
        if (chat) {
            setTimeout(() => { chat.scrollTop = chat.scrollHeight; }, 50);
        }
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

    /*initFadeScroll: function () {
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
    },*/
    initFadeScroll: function () {
    const elements = document.querySelectorAll('section, .timeline-item');

    elements.forEach((el, index) => {
        // Dynamique : pair/impair
        const isEven = (index + 1) % 2 === 0;
        el.classList.add(isEven ? 'slide-left' : 'slide-right');

        const observer = new IntersectionObserver(entries => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    if (el.classList.contains('slide-left')) {
                        el.style.animation = 'slideInLeft 0.8s forwards';
                    } else {
                        el.style.animation = 'slideInRight 0.8s forwards';
                    }
                    observer.unobserve(el);
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
