import { Component, computed, inject, signal } from '@angular/core';
import { FeaturedProductResponse } from '../../../core/interfaces/featured-product-response.interface';
import { FeaturedProductService } from '../../../core/services/featured-product.service';
import { PagedFeaturedProduct } from '../../../core/interfaces/paged-featured-product.interafce';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatProgressSpinner } from "@angular/material/progress-spinner";
import { MatCellDef, MatHeaderCellDef, MatHeaderRowDef, MatRowDef, MatTableModule } from "@angular/material/table";
import { MatIcon } from "@angular/material/icon";
import { NotificationService } from '../../../core/services/notification.service';

@Component({
  selector: 'app-featured-table',
  imports: [MatCellDef, MatHeaderCellDef, MatPaginator, MatRowDef, MatHeaderRowDef,
    MatProgressSpinner, MatTableModule, MatIcon],
  templateUrl: './featured-table.html',
  styleUrl: './featured-table.css',
})
export class FeaturedTable {


  protected displayedColumns: string[] = ['id', 'productId', 'productName', 'actions'];
  protected pageSize = signal(10);
  protected currentPage = signal(1);
  protected totalItems = signal(0);
  protected isLoading = signal(false);
  private allFeaturedProducts = signal<FeaturedProductResponse[]>([]);
  public pagedSubcategories = computed(() => this.allFeaturedProducts());
  private featuredProductService = inject(FeaturedProductService);


  private notify = inject(NotificationService);

  ngOnInit(): void {
    this.loadOrders();
  }

  private loadOrders() {
    this.isLoading.set(true);
    this.featuredProductService.getFeaturedProducts(this.currentPage(), this.pageSize())
      .subscribe({
        next: (response: PagedFeaturedProduct) => {
          this.allFeaturedProducts.set(response.data);
          this.totalItems.set(response.totalCount);
          this.isLoading.set(false);
        },
        error: (err) => {
          this.isLoading.set(false);
          this.allFeaturedProducts.set([]);
        }
      });
  }
  onPageChange(event: PageEvent): void {
    this.currentPage.set(event.pageIndex + 1);
    this.pageSize.set(event.pageSize);
    this.loadOrders();
  }

  removeFeatured(element: any) {
    const id = element.id; // Extraes el ID aquí
    this.featuredProductService.deleteFeaturedProduct(id).subscribe({
      next: (res) => {
        if (res.success) {
          this.notify.show("Producto eliminado.");
          this.loadOrders();
        }
      },
      error: (err) => this.notify.show("Ha ocurrido un error.")
    });
  }

}
