import { inject, Injectable, signal } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { NavigationEnd, Router } from '@angular/router';
import { filter, map } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class ToggleSlider {
  isOpen = signal(false);
  private router = inject(Router);


  toggle() {
    this.isOpen.update(valor => !valor);
  }

  close() {
    this.isOpen.set(false);
  }
  
  goToPage(routerResource: string) {
    this.router.navigate([routerResource]);
    this.close(); // Siempre cerrar al navegar
  }

  currentUrl = toSignal(
    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd),
      map(event => (event as NavigationEnd).urlAfterRedirects)
    ),
    { initialValue: this.router.url }
  );
}
