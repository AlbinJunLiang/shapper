import { inject, Injectable, signal } from '@angular/core'; // Importamos signal
import { TranslateService } from '@ngx-translate/core';
import { take } from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class LanguageService {
    private translate = inject(TranslateService);
    private readonly LANG_KEY = 'lang';

    // Creamos una señal para el idioma actual
    private _currentLang = signal<string>('es');
    // Exponemos la señal como de solo lectura para los componentes
    public currentLang = this._currentLang.asReadonly();

    constructor() {
        this.initLanguage();
    }

    private initLanguage() {
        const savedLang = localStorage.getItem(this.LANG_KEY);
        const browserLang = this.translate.getBrowserLang();
        const defaultLang = savedLang || (browserLang?.match(/en|es/) ? browserLang : 'es');
        this.changeLanguage(defaultLang!); 
    }

    changeLanguage(lang: string) {
        this.translate.use(lang).pipe(take(1)).subscribe({
            next: () => {
                localStorage.setItem(this.LANG_KEY, lang);
                this._currentLang.set(lang); // Actualizamos la señal
            },
            error: (err) => console.error('Error al cargar el idioma', err)
        });
    }
}