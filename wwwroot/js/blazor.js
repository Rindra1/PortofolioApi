function loadBlazorOptimized() {
            if (window.Blazor) return;
            const script = document.createElement('script');
            script.src = '/_framework/blazor.web.js';
            script.defer = true;
            script.async = true;
            script.fetchPriority = 'low';
            script.setAttribute('data-lazy', 'true');
            if (window.innerWidth < 768) {
                setTimeout(() => document.body.appendChild(script), 4000);
            } else {
                document.body.appendChild(script);
            }
            script.onload = function() {
                hideLoadingIndicator();
                //console.log('✅ Blazor chargé avec succès');
            };
        }

        function hideLoadingIndicator() {
            const loadingDiv = document.getElementById('blazor-loading');
            if (loadingDiv) {
                loadingDiv.style.transition = 'opacity 0.5s ease-out';
                loadingDiv.style.opacity = '0';
                setTimeout(() => {
                    if (loadingDiv && loadingDiv.parentNode) {
                        loadingDiv.parentNode.removeChild(loadingDiv);
                    }
                }, 500);
            }
        }

        let blazorLoadAttempted = false;
        function scheduleBlazorLoad() {
            if (blazorLoadAttempted) return;
            blazorLoadAttempted = true;
            if (document.readyState === 'complete') {
                requestAnimationFrame(() => {
                    if ('requestIdleCallback' in window) {
                        requestIdleCallback(loadBlazorOptimized);
                    } else {
                        setTimeout(loadBlazorOptimized, 3000);
                    }
                });
            } else {
                window.addEventListener('load', () => {
                    setTimeout(loadBlazorOptimized, 2000);
                });
            }
        }
        scheduleBlazorLoad();