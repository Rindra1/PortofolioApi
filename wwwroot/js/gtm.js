function initGTM() {
            if (localStorage.getItem('gtmConsent') !== 'accepted') {
                showCookieBanner();
                return;
            }
            (function(w,d,s,l,i){
                w[l]=w[l]||[];
                w[l].push({'gtm.start': new Date().getTime(), event:'gtm.js'});
                var f=d.getElementsByTagName(s)[0],
                    j=d.createElement(s),
                    dl=l!='dataLayer'?'&l='+l:'';
                j.async=true;
                j.src='https://www.googletagmanager.com/gtm.js?id='+i+dl;
                j.fetchPriority = (window.location.pathname === '/' || window.location.pathname === '/contact') ? 'high' : 'low';
                f.parentNode.insertBefore(j,f);
            })(window,document,'script','dataLayer','G-7D0HPW8VLE');
        }

        let gtmLoaded = false;
        function loadGTMWhenReady() {
            if (gtmLoaded) return;
            if (document.readyState === 'complete') {
                initGTM();
                gtmLoaded = true;
                return;
            }
            const events = ['scroll', 'mousemove', 'keydown', 'touchstart', 'click'];
            const loadOnInteraction = () => {
                if (!gtmLoaded) {
                    initGTM();
                    gtmLoaded = true;
                }
                events.forEach(e => document.removeEventListener(e, loadOnInteraction));
            };
            events.forEach(e => document.addEventListener(e, loadOnInteraction, { passive: true }));
            setTimeout(() => {
                if (!gtmLoaded) {
                    initGTM();
                    gtmLoaded = true;
                }
            }, 4000);
        }

        if ('requestIdleCallback' in window) {
            requestIdleCallback(loadGTMWhenReady);
        } else {
            setTimeout(loadGTMWhenReady, 2000);
        }