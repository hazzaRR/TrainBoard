export const intToHex = (colourInt) => {
    return `#${(Number(colourInt).toString(16)).padStart(6, '0')}`;
}

export const hexToInt = (hexString) => {
    hexString = hexString.replace('#', '');
    return parseInt(hexString, 16);
}