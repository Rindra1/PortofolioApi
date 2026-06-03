
window.siteInterop = {
    hideLoader: function() {
        const loader = document.getElementById('loader');
        if(loader) loader.style.display = 'none';
    },

    initAll: function () {
        AOS.init({ duration: 800, once: true, offset: 100 });

    // Theme
    const themeToggle = document.getElementById('themeToggle');
    const body = document.body;
    const themeIcon = themeToggle.querySelector('i');
    function setTheme(theme) {
        if(theme === 'dark') {
            body.classList.remove('light'); body.classList.add('dark');
            localStorage.setItem('theme', 'dark');
            themeIcon.classList.remove('fa-moon'); themeIcon.classList.add('fa-sun');
        } else {
            body.classList.remove('dark'); body.classList.add('light');
            localStorage.setItem('theme', 'light');
            themeIcon.classList.remove('fa-sun'); themeIcon.classList.add('fa-moon');
        }
    }
    if(localStorage.getItem('theme') === 'dark') setTheme('dark'); else setTheme('light');
    themeToggle.addEventListener('click', () => setTheme(body.classList.contains('light') ? 'dark' : 'light'));

    // Mobile menu
    const mobileBtn = document.getElementById('mobileMenuBtn');
    const mobileNav = document.getElementById('mobileNav');
    mobileBtn.addEventListener('click', () => mobileNav.classList.toggle('show'));
    document.querySelectorAll('.mobile-nav a').forEach(link => link.addEventListener('click', () => mobileNav.classList.remove('show')));

    // Download CV
    //document.querySelectorAll('#downloadCVBtn, #downloadCVBtnMobile').forEach(btn => 
        //btn.addEventListener('click', (e) => { e.preventDefault(); alert("📄 Le CV sera disponible prochainement. Merci de votre intérêt !"); })
    //);

    // Gallery functions for each project
    function initGallery(galleryId, mainImageId) {
        console.log(`Initializing gallery: ${galleryId} with main image: ${mainImageId}`);
        const thumbnails = document.querySelectorAll(`#${galleryId} .thumbnail`);
        const mainImage = document.getElementById(mainImageId);
        thumbnails.forEach(thumb => {
            thumb.addEventListener('click', function() {
                mainImage.src = this.getAttribute('data-img');
                thumbnails.forEach(t => t.classList.remove('active'));
                this.classList.add('active');
            });
            // Activate first thumbnail by default
            if(thumb === thumbnails[0]) thumb.classList.add('active');
        });
    }

    document.addEventListener('click', function(e) {

        if (!e.target.classList.contains('thumbnail'))
            return;

    const gallery = e.target.closest('.modal-gallery');
    const mainImage = gallery.querySelector('.main-image');

    mainImage.src = e.target.dataset.img;

    gallery.querySelectorAll('.thumbnail')
        .forEach(t => t.classList.remove('active'));

    e.target.classList.add('active');
    });

    initGallery('gallery1', 'mainImage1');
    initGallery('gallery2', 'mainImage2');
    initGallery('gallery3', 'mainImage3');

    // Modal functions
    window.openModal = function(projetId) {
        document.getElementById(`modal-${projetId}`).style.display = 'flex';
    }
    window.closeModal = function(projetId) {
        document.getElementById(`modal-${projetId}`).style.display = 'none';
    }
    window.onclick = function(event) {
        if(event.target.classList.contains('modal')){
            console.log('Clicked outside modal content, closing modal');
            event.target.style.display = 'none';
        } 
    }

    // Contact form
    const contactForm = document.getElementById('contactForm');
    const formStatus = document.getElementById('formStatus');
    contactForm.addEventListener('submit', function(e) {
        e.preventDefault();
        const name = document.getElementById('userName').value;
        const email = document.getElementById('userEmail').value;
        const message = document.getElementById('userMessage').value;
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

    // Chatbot
    const toggleBtn = document.getElementById('chatbotToggle');
    const chatWindow = document.getElementById('chatWindow');
    const closeChat = document.getElementById('closeChat');
    document.addEventListener('click', function (e) {
        console.log('Clicked element:', e.target);
    if (e.target.closest('#chatbotToggle')) {
        
            const chat = document.getElementById('chatWindow');
            chat.classList.toggle('open');

console.log(chat.className);
console.log(getComputedStyle(chat).opacity);
console.log(getComputedStyle(chat).transform);
console.log(getComputedStyle(chat).display);
    }

    if (e.target.closest('#closeChat')) {
        document.getElementById('chatWindow')
            ?.classList.remove('open');
    }
});
    //toggleBtn.addEventListener('click', () => chatWindow.classList.toggle('open'));
    //closeChat.addEventListener('click', () => chatWindow.classList.remove('open'));
    /*
    const chatDiv = document.getElementById('chatMessagesFloat');
    const chatInput = document.getElementById('chatInputFloat');
    const sendBtn = document.getElementById('sendChatFloat');
    function addMessage(text, isUser = false) {
        const div = document.createElement('div');
        if(isUser) div.className = 'message-user', div.innerHTML = `<div class="user-bubble">${text}</div>`;
        else div.className = 'message-bot', div.innerHTML = `<div class="bot-avatar">🤖</div><div class="bot-bubble">${text}</div>`;
        chatDiv.appendChild(div);
        chatDiv.scrollTop = chatDiv.scrollHeight;
    }
    function getReply(q) {
        const question = q.toLowerCase();
        if(question.includes('projet')) return "Rindra a développé plusieurs apps Blazor, WinForms et APIs REST. Cliquez sur 'Détails' pour voir les images !";
        if(question.includes('stack') || question.includes('technologie')) return "Sa stack : C#/.NET, ASP.NET Core, Blazor, VB.NET, SQL Server, EF, JWT, Docker.";
        if(question.includes('disponible') || question.includes('freelance')) return "Oui, disponible pour missions freelance ou CDI. Contactez-le via le formulaire !";
        return "Je peux vous parler de ses projets .NET, sa stack technique ou sa disponibilité.";
    }
    function handleSend() {
        const msg = chatInput.value.trim();
        if(!msg) return;
        addMessage(msg, true);
        chatInput.value = '';
        setTimeout(() => addMessage(getReply(msg), false), 400);
    }
    sendBtn.addEventListener('click', handleSend);
    chatInput.addEventListener('keypress', e => { if(e.key === 'Enter') handleSend(); });
    document.querySelectorAll('.suggestion-chip').forEach(chip => {
        chip.addEventListener('click', () => { chatInput.value = chip.getAttribute('data-suggestion'); handleSend(); });
    });*/
        // Masquer le loader après tout
        this.hideLoader();
    },

    initKeepAlive: function () {

    // évite double interval (Blazor re-render / init multiple)
    if (this._keepAliveStarted) return;
    this._keepAliveStarted = true;

    console.log("Keep-alive activé");

    setInterval(() => {
        fetch("https://ton-api.onrender.com/api/health")
            .then(() => console.log("keep-alive OK"))
            .catch(err => console.log("keep-alive error", err));
    }, 4 * 60 * 1000); // 4 minutes
},


    
};



    // ⚡ Initialisation automatique après que le DOM est chargé
//window.addEventListener("DOMContentLoaded", () => {
    //window.siteInterop.initAll();
//});