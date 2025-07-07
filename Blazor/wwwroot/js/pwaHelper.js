let deferredPrompt = null;

window.addEventListener('beforeinstallprompt', (e) => {
    e.preventDefault();
    deferredPrompt = e;
});

export function canPromptInstall() {
    return deferredPrompt !== null;
}

export async function showInstallPrompt() {
    if (deferredPrompt) {
        deferredPrompt.prompt();
        const { outcome } = await deferredPrompt.userChoice;
        deferredPrompt = null;
        return outcome === 'accepted';
    }
    return false;
}

export function isPWAInstalled() {
    return window.matchMedia('(display-mode: standalone)').matches || window.navigator.standalone === true;
}

export function getIsOnline() {
    return navigator.onLine;
} 