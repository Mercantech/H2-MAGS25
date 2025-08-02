const key = "HejH2Hemmelighed"; // 16 tegn

// Sikkerhedstjek for CryptoJS
function ensureCryptoJS() {
    if (typeof CryptoJS === 'undefined') {
        throw new Error('CryptoJS er ikke indlæst. Sørg for at CryptoJS CDN er inkluderet i index.html');
    }
}

export function setEncryptedItem(storageKey, value) {
    ensureCryptoJS();
    const encrypted = CryptoJS.AES.encrypt(value, key).toString();
    localStorage.setItem(storageKey, encrypted);
}

export function getDecryptedItem(storageKey) {
    ensureCryptoJS();
    const encrypted = localStorage.getItem(storageKey);
    if (!encrypted) return null;
    const bytes = CryptoJS.AES.decrypt(encrypted, key);
    return bytes.toString(CryptoJS.enc.Utf8);
}

export function setItem(key, value) {
    localStorage.setItem(key, value);
}

export function getItem(key) {
    return localStorage.getItem(key);
}

export function removeItem(key) {
    localStorage.removeItem(key);
}

// JWT funktionalitet
export function setJWTToken(token) {
    ensureCryptoJS();
    const encrypted = CryptoJS.AES.encrypt(token, key).toString();
    localStorage.setItem('jwt_token', encrypted);
}

export function getJWTToken() {
    ensureCryptoJS();
    const encrypted = localStorage.getItem('jwt_token');
    if (!encrypted) return null;
    const bytes = CryptoJS.AES.decrypt(encrypted, key);
    return bytes.toString(CryptoJS.enc.Utf8);
}

export function removeJWTToken() {
    localStorage.removeItem('jwt_token');
}

export function decodeJWT(token) {
    try {
        const base64Url = token.split('.')[1];
        const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
        const jsonPayload = decodeURIComponent(atob(base64).split('').map(function(c) {
            return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
        }).join(''));
        return JSON.parse(jsonPayload);
    } catch (e) {
        return null;
    }
} 