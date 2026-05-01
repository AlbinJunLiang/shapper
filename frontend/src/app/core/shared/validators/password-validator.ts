import { AbstractControl, } from '@angular/forms';

export const matchValidator = (g: AbstractControl) => {
    const p1 = g.get('password')?.value;
    const p2 = g.get('confirmPassword')?.value;
    return (p1 && p2 && p1 !== p2) ? { mustMatch: true } : null;
};