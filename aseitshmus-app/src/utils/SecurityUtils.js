import CryptoJS from 'crypto-js';

export function hashText(value) {
    if (value) {
        const hash = CryptoJS.SHA256(value);
        return hash.toString(CryptoJS.enc.Hex);
    }
    return null;
}
