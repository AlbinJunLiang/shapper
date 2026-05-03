import { Component, computed, inject } from '@angular/core';
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
import { MatProgressSpinner } from "@angular/material/progress-spinner";

@Component({
  selector: 'app-product-list',
  imports: [CommonModule, RouterOutlet, RouterLinkWithHref, RouterLink, Searcher, TranslateModule, MatProgressSpinner],
  templateUrl: './product-list.html'
})
export class ProductList {

  protected currency = environment.currency;
  public productStore = inject(ProductStore);

  private productDialogService = inject(ProductDialogService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);

  public totalItems = computed(() =>
    this.productStore.apiResponse().totalCount
  );

  public totalPages = computed(() =>
    this.productStore.apiResponse().totalPages
  );

  readonly pageSize = 8;
  protected Status = ProductStatus;

  public productsWithTax = computed(() => {

    return this.productStore.products().map(product => ({
      ...product,
      totalDisplayPrice: product.price * (1 + (product.taxAmount || 0) / 100)
    }));
  });

  public currentPage = computed(() =>
    this.productStore.apiResponse().page
  );

  private currentParams: any = {};

  ngOnInit() {
    if (this.getFilterPath() === "all") {
      this.loadProducts(this.currentPage(), this.pageSize);
    } else {
      this.route.queryParams.subscribe(params => {
        this.currentParams = params;
        this.filterProducts(params, 1, this.pageSize);
      });
    }
  }

  getFilterPath() {
    return getPathFromSegment(this.router.url, 'filter') || "all";
  }

  loadProducts(page: number, size: number) {
    this.productStore.loadProducts(page, size, false);
  }

  filterProducts(queryParams: any, page: number, size: number) {
    if (Object.keys(queryParams).length > 0) {
      this.productStore.loadFilterProducts(queryParams, page, size);
    }
  }

  startItem() {
    if (this.totalItems() === 0) return 0;
    return (this.currentPage() - 1) * this.pageSize + 1;
  }

  endItem() {
    return Math.min(this.currentPage() * this.pageSize, this.totalItems());
  }



  nextPage() {
    if (this.currentPage() < this.totalPages()) {
      if (this.getFilterPath() === "all") {
        this.loadProducts(this.currentPage() + 1, this.pageSize);
      } else {
        this.filterProducts(this.currentParams, this.currentPage() + 1, this.pageSize);
      }
      scrollToTop();
    }
  }

  prevPage() {
    if (this.currentPage() > 1) {
      if (this.getFilterPath() === "all") {
        this.loadProducts(this.currentPage() - 1, this.pageSize);
      } else {
        this.filterProducts(this.currentParams, this.currentPage() - 1, this.pageSize);
      }
      scrollToTop();
    }
  }

  showProduct(product: Product) {
    this.productDialogService.openDetail(product);
  }
}