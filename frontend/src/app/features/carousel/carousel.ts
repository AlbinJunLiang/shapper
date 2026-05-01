import { Component, ElementRef, inject, signal, viewChild } from '@angular/core';
import { ProductDialogService } from '../../core/services/product-dialog.service';
import { Product } from '../../core/interfaces/product.interface';
import { Searcher } from "../searcher/searcher";
import { ProductService } from '../../core/services/product-service';
import { ProductStore } from '../../core/services/product-store';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-carousel',
  imports: [Searcher, TranslateModule],
  templateUrl: './carousel.html',
  styleUrl: './carousel.css',
})
export class Carousel {

  private scrollAmount = 320;
  private autoScrollInterval?: any;
  private productDialogService = inject(ProductDialogService);

  public products = signal<Product[]>([]);
  public isLoading = signal<boolean>(false);

  protected productService = inject(ProductService);
  private productStore = inject(ProductStore); // Cargar unos datos para el searcher
  protected carousel = viewChild<ElementRef<HTMLDivElement>>('carouselRef');

  ngOnInit() {
    this.startAutoScroll();
    this.loadFeatured();
  }

  loadFeatured() {
    this.isLoading.set(true);
    this.productService.getProductsStore(1, 20, true).subscribe({
      next: (response) => {
        this.products.set(response.data);
        this.productStore.updateLocalProducts(response.data)
      },
      error: (err) => {
        console.error('Error:', err);
      },
      complete: () => {
        this.isLoading.set(false);
      }
    });
  }

  protected scroll(direction: number) {
    const el = this.carousel()?.nativeElement;
    if (!el) return;
    const scrollWidth = el.scrollWidth;
    const clientWidth = el.clientWidth;
    const currentScroll = el.scrollLeft;

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
    if (this.autoScrollInterval) {
      clearInterval(this.autoScrollInterval);
    }
  }

  protected resumeAutoScroll() {
    this.startAutoScroll();
  }

  protected showProduct(product: Product) {
    this.productDialogService.openDetail(product);
  }

  protected ngOnDestroy() {
    clearInterval(this.autoScrollInterval);
  }

  private startAutoScroll() {
    if (this.autoScrollInterval) clearInterval(this.autoScrollInterval);

    this.autoScrollInterval = setInterval(() => {
      const el = this.carousel()?.nativeElement;
      if (!el) return;

      if (el.scrollLeft + el.clientWidth >= el.scrollWidth - 10) {
        el.scrollTo({ left: 0, behavior: 'smooth' });
      } else {
        el.scrollBy({ left: this.scrollAmount, behavior: 'smooth' });
      }
    }, 4000);
  }
  private resetAutoScroll() {
    clearInterval(this.autoScrollInterval);
    this.startAutoScroll();
  }
} //END
