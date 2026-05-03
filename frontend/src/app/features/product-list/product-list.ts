import { Component, computed, inject, signal } from '@angular/core';
import { ProductService } from '../../core/services/product-service';
import { ProductDialogService } from '../../core/services/product-dialog.service';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterOutlet, RouterLinkWithHref, RouterLink } from '@angular/router';
import { Product } from '../../core/interfaces/product.interface';
import { ProductStore } from '../../core/services/product-store';
import { getPathFromSegment } from '../../core/shared/utils/url-util';
import { scrollToTop } from '../../core/shared/utils/scroll-util';
import { Searcher } from "../searcher/searcher";
import { ProductStatus } from '../../core/enums/product-status.enum';
import { TranslateModule } from '@ngx-translate/core';
import { environment } from '../../../environments/environment.development';

@Component({
  selector: 'app-product-list',
  imports: [CommonModule, RouterOutlet, RouterLinkWithHref, RouterLink, Searcher, TranslateModule],
  templateUrl: './product-list.html'
})
export class ProductList {

  protected currency = environment.currency;
  private productService = inject(ProductService);
  public productStore = inject(ProductStore);

  private productDialogService = inject(ProductDialogService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);

  public products = signal<Product[]>([]);
  public currentPage = signal(1);
  public totalItems = signal(0);
  readonly pageSize = 8;
  protected Status = ProductStatus;

public productsWithTax = computed(() => {
  return this.productStore.products().map(product => ({
    ...product,
    // Calculamos el total una sola vez por producto
    totalDisplayPrice: product.price * (1 + (product.taxAmount || 0) / 100)
  }));
});

  get totalPages() {
    return Math.ceil(this.totalItems() / this.pageSize);
  }

  ngOnInit() {
    if (this.getFilterPath() === "all") {
      this.loadProducts(this.currentPage(), this.pageSize);
    } else {
      this.route.queryParams.subscribe(params => {
        if (Object.keys(params).length > 0) {
          this.filterProducts((params));
        }
      });
    }
  }

  getFilterPath() {
    return getPathFromSegment(this.router.url, 'filter') || "all";
  }

  //
  loadProducts(page: number, size: number) {
    this.productService.getProductsStore(page, size, false)
      .subscribe({
        next: (data) => {
          this.totalItems.set(data.totalCount);
          this.products.set(data.data);
          this.productStore.updateLocalProducts(data.data)
        },
        error: (err) => console.error("Error:", err)
      });
  }

  filterProducts(queryParams: any) {
    this.productService.filterProducts(queryParams)
      .subscribe({
        next: (data) => {
          this.totalItems.set(data.totalCount);
          this.products.set(data.data);
          this.productStore.updateLocalProducts(data.data)

        },
        error: (err) => console.error("Error:", err)
      });
  }


  startItem() {
    if (this.totalItems() === 0) return 0;
    return (this.currentPage() - 1) * this.pageSize + 1;
  }

  endItem() {
    return Math.min(this.currentPage() * this.pageSize, this.totalItems());
  }

  nextPage() {
    if (this.currentPage() < this.totalPages) {
      this.currentPage.update(p => p + 1);
      this.loadProducts(this.currentPage(), this.pageSize);
      scrollToTop();
    }
  }

  prevPage() {
    if (this.currentPage() > 1) {
      this.currentPage.update(p => p - 1);
      this.loadProducts(this.currentPage(), this.pageSize);
      scrollToTop();
    }
  }

  showProduct(product: Product) {
    this.productDialogService.openDetail(product);
  }
}