function showCookieBanner() {
            if (document.getElementById('cookie-banner')) return;
            const banner = document.createElement('div');
            banner.id = 'cookie-banner';
            banner.style.cssText = `
                position: fixed; bottom: 20px; left: 50%; transform: translateX(-50%);
                background: #1a1a2e; color: white; padding: 1rem 2rem;
                border-radius: 12px; z-index: 9999;
                box-shadow: 0 4px 20px rgba(0,0,0,0.5);
                max-width: 90%; text-align: center;
                font-family: 'Inter', sans-serif;
                border: 1px solid rgba(255,255,255,0.1);
            `;
            banner.innerHTML = `
                <p style="margin: 0 0 0.5rem 0; font-size: 0.9rem;">
                    🍪 J'utilise des cookies pour améliorer votre expérience.
                </p>
                <button onclick="acceptGTMCookies()" style="
                    background: #6C63FF; border: none; color: white;
                    padding: 0.5rem 1.5rem; border-radius: 8px;
                    cursor: pointer; font-weight: 600; margin: 0 0.5rem;
                ">
                    Accepter
                </button>
                <button onclick="dismissCookieBanner()" style="
                    background: transparent; border: 1px solid rgba(255,255,255,0.3);
                    color: white; padding: 0.5rem 1.5rem; border-radius: 8px;
                    cursor: pointer;
                ">
                    Refuser
                </button>
            `;
            document.body.appendChild(banner);
        }

        function acceptGTMCookies() {
            localStorage.setItem('gtmConsent', 'accepted');
            document.getElementById('cookie-banner')?.remove();
            initGTM();
        }

        function dismissCookieBanner() {
            localStorage.setItem('gtmConsent', 'refused');
            document.getElementById('cookie-banner')?.remove();
        }