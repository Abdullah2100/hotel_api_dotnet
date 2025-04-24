
export function isHasCapitalLetter(text: string) {
    const regex = /(.*[A-Z].*[A-Z])/;
    return regex.test(text);
}

export function isHasSmallLetter(text: string) {
    const regex = /(.*[a-z].*[a-z])/;
    return regex.test(text);
}

export function isHasSpicalCharacter(text: string) {
    // Regex to check if there are at least 2 special characters
    const regex = /[!@#$%^&*()_+|\\/?<>,.:;'\"-].*[!@#$%^&*()_+|\\/?<>,.:;'\"-]/;
    return regex.test(text);
}

export function isHasNumber(text: string) {
    const regex = /(.*[0-9].*[0-9])/;
    return regex.test(text);
}

export function isValidEmail(text:string){
    const regex = /[\w-\.]+@([\w-]+\.)+[\w-]{2,4}/
    return regex.test;
}
