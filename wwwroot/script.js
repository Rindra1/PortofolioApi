window.siteInterop = {
    hideById: function(id) {
        try {
            const el = document.getElementById(id);
            if (el) el.style.display = 'none';
        } catch (e) { /* ignore */ }
    },
    hideLoader: function() {
        this.hideById('loader');
    },
    scrollToSection: function(sectionId) {
        const element = document.getElementById(sectionId);
        if (element) {
            element.scrollIntoView({ behavior: 'smooth' });
            // Optionnel : nettoyer l'URL
            history.pushState(null, null, window.location.pathname);
        }
    },
    
    closeMobileNav: function() {
        const mobileNav = document.getElementById('mobileNav');
        if (mobileNav) {
            mobileNav.classList.remove('active');
        }
    },

    initAll: function () {
        // Initialisation AOS
        if (typeof AOS !== 'undefined') {
            AOS.init({ duration: 800, once: false, mirror: true, offset: 100 });
        }

        // Theme
        const themeToggle = document.getElementById('themeToggle');
        if (themeToggle) {
            const body = document.body;
            const themeIcon = themeToggle.querySelector('i');
            function setTheme(theme) {
                if(theme === 'dark') {
                    body.classList.remove('light'); body.classList.add('dark');
                    localStorage.setItem('theme', 'dark');
                    if(themeIcon) {
                        themeIcon.classList.remove('fa-moon'); 
                        themeIcon.classList.add('fa-sun');
                    }
                } else {
                    body.classList.remove('dark'); body.classList.add('light');
                    localStorage.setItem('theme', 'light');
                    if(themeIcon) {
                        themeIcon.classList.remove('fa-sun'); 
                        themeIcon.classList.add('fa-moon');
                    }
                }
            }
            if(localStorage.getItem('theme') === 'dark') setTheme('dark'); 
            else setTheme('light');
            themeToggle.addEventListener('click', () => setTheme(body.classList.contains('light') ? 'dark' : 'light'));
        }

        // Mobile menu
        this.initMobileMenu();

        // Gallery functions
        this.initGalleries();

        // Modal functions
        this.initModals();

        // Chatbot
        this.initChatbot();

        this.hideLoader();
    },

    initMobileMenu: function() {
        const mobileMenuBtn = document.getElementById('mobileMenuBtn');
        const mobileNav = document.getElementById('mobileNav');
        
        if (mobileMenuBtn && mobileNav) {
            // Fonction pour fermer le menu
            const closeMenu = () => {
                mobileNav.classList.remove('show');
                const icon = mobileMenuBtn.querySelector('i');
                if (icon) {
                    icon.classList.remove('fa-times');
                    icon.classList.add('fa-bars');
                }
            };
            
            // Fonction pour ouvrir/fermer le menu
            const toggleMenu = (e) => {
                e.stopPropagation();
                e.preventDefault();
                mobileNav.classList.toggle('show');
                const icon = mobileMenuBtn.querySelector('i');
                if (mobileNav.classList.contains('show')) {
                    icon.classList.remove('fa-bars');
                    icon.classList.add('fa-times');
                } else {
                    icon.classList.remove('fa-times');
                    icon.classList.add('fa-bars');
                }
            };
            
            // Écouteur sur le bouton
            mobileMenuBtn.removeEventListener('click', toggleMenu);
            mobileMenuBtn.addEventListener('click', toggleMenu);
            
            // Fermer le menu en cliquant sur un lien
            const links = mobileNav.querySelectorAll('a');
            links.forEach(link => {
                link.removeEventListener('click', closeMenu);
                link.addEventListener('click', closeMenu);
            });
            
            // Fermer le menu en cliquant en dehors
            document.removeEventListener('click', function(event) {
                if (mobileNav && mobileMenuBtn && !mobileNav.contains(event.target) && !mobileMenuBtn.contains(event.target)) {
                    if (mobileNav.classList.contains('show')) {
                        closeMenu();
                    }
                }
            });
            document.addEventListener('click', function(event) {
                if (mobileNav && mobileMenuBtn && !mobileNav.contains(event.target) && !mobileMenuBtn.contains(event.target)) {
                    if (mobileNav.classList.contains('show')) {
                        closeMenu();
                    }
                }
            });
        }
    },

    initGalleries: function () {
    document.addEventListener('click', function (e) {
        const thumb = e.target.closest('.thumbnail');
        if (!thumb) return;

        const gallery = thumb.closest('[id^="gallery"]');
        if (!gallery) return;

        const mainImage = gallery.querySelector('.main-image');
        const thumbnails = gallery.querySelectorAll('.thumbnail');

        const imgSrc = thumb.getAttribute('data-img');
        if (imgSrc && mainImage) {
            mainImage.src = imgSrc;
        }

        thumbnails.forEach(t => t.classList.remove('active'));
        thumb.classList.add('active');
    });
},

    initModals: function() {
        window.openModal = function(projetId) {
            const modal = document.getElementById(`modal-${projetId}`);
            if(modal) modal.style.display = 'flex';
        };
        window.closeModal = function(projetId) {
            const modal = document.getElementById(`modal-${projetId}`);
            if(modal) modal.style.display = 'none';
        };
        window.onclick = function(event) {
            if(event.target.classList && event.target.classList.contains('modal')){
                event.target.style.display = 'none';
            } 
        };
    },

    initContactForm: function() {
        const contactForm = document.getElementById('contactForm');
        const formStatus = document.getElementById('formStatus');
        if(contactForm && formStatus) {
            contactForm.removeEventListener('submit', function() {});
            contactForm.addEventListener('submit', function(e) {
                e.preventDefault();
                const name = document.getElementById('userName')?.value;
                const email = document.getElementById('userEmail')?.value;
                const message = document.getElementById('userMessage')?.value;
                if(!name || !email || !message) {
                    formStatus.className = 'form-message error';
                    formStatus.textContent = '❌ Veuillez remplir tous les champs.';
                    formStatus.style.display = 'block';
                    return;
                }
                formStatus.className = 'form-message';
                formStatus.textContent = '📤 Envoi en cours...';
                formStatus.style.display = 'block';
                setTimeout(() => {
                    formStatus.className = 'form-message success';
                    formStatus.textContent = '✅ Message envoyé avec succès ! Je vous répondrai rapidement.';
                    contactForm.reset();
                }, 1500);
            });
        }
    },

    initChatbot: function() {
        const toggleBtn = document.getElementById('chatbotToggle');
        const chatWindow = document.getElementById('chatWindow');
        const closeChat = document.getElementById('closeChat');
        
        if (toggleBtn && chatWindow) {
            const toggleChat = (e) => {
                if(e) e.stopPropagation();
                chatWindow.classList.toggle('open');
            };
            toggleBtn.removeEventListener('click', toggleChat);
            toggleBtn.addEventListener('click', toggleChat);
            
            if (closeChat) {
                const closeChatWindow = () => {
                    chatWindow.classList.remove('open');
                };
                closeChat.removeEventListener('click', closeChatWindow);
                closeChat.addEventListener('click', closeChatWindow);
            }
        }
    },

initKeepAlive: function () {
        if (this._keepAliveStarted) return;
        this._keepAliveStarted = true;
        console.log("Keep-alive activé");
        setInterval(() => {
            fetch("/api/health")
                .then(() => console.log("keep-alive OK"))
                .catch(err => console.log("keep-alive error", err));
        }, 4 * 60 * 1000);
    }
};

// Initialisation automatique
if (document.readyState === 'loading') {
    document.addEventListener("DOMContentLoaded", function() {
        if (window.siteInterop) {
            window.siteInterop.initAll();
        }
    });
} else {
    if (window.siteInterop) {
        window.siteInterop.initAll();
    }
}

window.addEventListener('load', function() {
    if (window.siteInterop) {
        window.siteInterop.hideLoader();
    }
});
