import { Component, computed, inject, input, signal } from '@angular/core';
import { MatProgressSpinner } from "@angular/material/progress-spinner";
import { MatPaginator, PageEvent } from "@angular/material/paginator";
import { MatHeaderCellDef, MatCellDef, MatTableModule } from '@angular/material/table';
import { TranslateModule } from '@ngx-translate/core';
import { CommonModule } from '@angular/common';
import { OrderService } from '../../../core/services/order-service';
import { getStatusTranslationKey } from '../../../core/shared/utils/order-status';
import { RouterLink } from "@angular/router";
import { OrderResponse } from '../../../core/interfaces/order-data.interface';
import { OrderDetail } from '../../../core/interfaces/order-detail.interface';
import { NotificationService } from '../../../core/services/notification.service';

@Component({
  selector: 'app-order-detail-table',
  imports: [CommonModule, MatHeaderCellDef, MatCellDef, TranslateModule, MatPaginator,
     MatProgressSpinner, MatTableModule, RouterLink],
  templateUrl: './order-detail-table.html',
  styleUrl: './order-detail-table.css',
})
export class OrderDetailTable {
  public reference = input<string>("");
  private readonly orderService = inject(OrderService);
  private notify = inject(NotificationService);
  public order = signal<OrderResponse | undefined | null>(undefined);
  public isLoading = signal(false);
  private allDetails = signal<OrderDetail[]>([]);
  protected orderStatus = getStatusTranslationKey;

  protected totalItems = signal(0);
  protected pageSize = signal(30);
  protected currentPage = signal(1);

  public pagedDetails = computed(() => {
    const startIndex = (this.currentPage() - 1) * this.pageSize();
    const endIndex = startIndex + this.pageSize();
    return this.allDetails().slice(startIndex, endIndex);
  });

  protected displayedColumns: string[] = ['productId', 'productName', 
    'description', 'requestedQuantity', 'actualQuantity', 'basePrice', 'subtotal'];
  protected show = false;
  public statuses = ['PAID', 'PENDING', 'CANCELLED', 'COMPLETED'];

  ngOnInit(): void {
    this.loadOrders();
  }

  private loadOrders() {
    this.isLoading.set(true);
    this.orderService.getOrderByReference(this.reference())
      .subscribe({
        next: (data: OrderResponse) => {
          this.order.set(data);
          if (data?.details) {
            this.allDetails.set(data.details);
            this.totalItems.set(data.details.length);
          }
          this.isLoading.set(false);
        },
        error: () => {
          this.order.set(null);
          this.isLoading.set(false);
        }
      });
  }

  onPageChange(event: PageEvent) {
    this.pageSize.set(event.pageSize);
    this.currentPage.set(event.pageIndex + 1);
  }


  changeStatus(newStatus: string) {
    const orderData = this.order();
    if (!orderData || !orderData.id) return;

    this.orderService.updateOrderStatus(orderData.id, newStatus)
      .subscribe({
        next: (response) => {
          if (response.success) {
            this.order.update(prev => prev ? { ...prev, status: newStatus } : null);
            this.notify.show("Se ha actualizado la orden.");
          }
        },
        error: (err) => {
          this.notify.show("Ha ocurrido un error.");
        }
      });
  }
}