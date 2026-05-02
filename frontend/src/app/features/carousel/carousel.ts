import { Component, computed, ElementRef, inject, viewChild, NgZone, OnDestroy, OnInit } from '@angular/core';
import { ProductDialogService } from '../../core/services/product-dialog.service';
import { Product } from '../../core/interfaces/product.interface';
import { Searcher } from "../searcher/searcher";
import { ProductService } from '../../core/services/product-service';
import { TranslateModule } from '@ngx-translate/core';
import { HomeStore } from '../../core/services/home-store';

@Component({
  selector: 'app-carousel',
  standalone: true, // Asegúrate de que esté marcado si usas imports
  imports: [Searcher, TranslateModule],
  templateUrl: './carousel.html',
  styleUrl: './carousel.css',
})
export class Carousel implements OnInit, OnDestroy {
  // Servicios e Inyecciones
  private ngZone = inject(NgZone);
  private productDialogService = inject(ProductDialogService);
  private homeStore = inject(HomeStore);
  protected productService = inject(ProductService);

  // Configuración y Estado
  private scrollAmount = 320;
  private autoScrollInterval?: ReturnType<typeof setInterval>;
  
  // Referencias y Signals
  protected carousel = viewChild<ElementRef<HTMLDivElement>>('carouselRef');
  public products = computed(() => this.homeStore.featuredProducts());

  ngOnInit() {
    this.startAutoScroll();
  }

  protected scroll(direction: number) {
    const el = this.carousel()?.nativeElement;
    if (!el) return;

    const scrollWidth = el.scrollWidth;
    const clientWidth = el.clientWidth;
    const currentScroll = el.scrollLeft;

    // Lógica de scroll infinito visual
    if (direction === 1 && (currentScroll + clientWidth >= scrollWidth - 10)) {
      el.scrollTo({ left: 0, behavior: 'smooth' });
    }
    else if (direction === -1 && currentScroll <= 10) {
      el.scrollTo({ left: scrollWidth, behavior: 'smooth' });
    }
    else {
      el.scrollBy({ left: direction * this.scrollAmount, behavior: 'smooth' });
    }

    this.resetAutoScroll();
  }

  protected pauseAutoScroll() {
    this.stopInterval();
  }

  protected resumeAutoScroll() {
    this.startAutoScroll();
  }

  protected showProduct(product: Product) {
    this.productDialogService.openDetail(product);
  }

  ngOnDestroy() {
    this.stopInterval();
  }

  private startAutoScroll() {
    this.stopInterval();

    // Ejecutamos fuera de Angular para que el "tick" del setInterval
    // no dispare la detección de cambios en toda la app cada 4 segundos.
    this.ngZone.runOutsideAngular(() => {
      this.autoScrollInterval = setInterval(() => {
        const el = this.carousel()?.nativeElement;
        if (!el) return;

        if (el.scrollLeft + el.clientWidth >= el.scrollWidth - 10) {
          el.scrollTo({ left: 0, behavior: 'smooth' });
        } else {
          el.scrollBy({ left: this.scrollAmount, behavior: 'smooth' });
        }
      }, 4000);
    });
  }

  private stopInterval() {
    if (this.autoScrollInterval) {
      clearInterval(this.autoScrollInterval);
      this.autoScrollInterval = undefined;
    }
  }

  private resetAutoScroll() {
    this.startAutoScroll();
  }
}