const key = "HejH2Hemmelighed"; // 16 tegn

export function setEncryptedItem(storageKey, value) {
    const encrypted = CryptoJS.AES.encrypt(value, key).toString();
    localStorage.setItem(storageKey, encrypted);
}

export function getDecryptedItem(storageKey) {
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