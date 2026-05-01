import { Component, computed, inject, signal } from '@angular/core';
import { OrderPayment } from '../../../core/interfaces/order-payment.interface';
import { OrderPaymentService } from '../../../core/services/order-payment.service';
import { PagedOrderPayment } from '../../../core/interfaces/paged-order-payment.interface';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatCellDef, MatHeaderCellDef, MatHeaderRowDef, MatRowDef, MatTableModule } from '@angular/material/table';
import { MatProgressSpinner } from '@angular/material/progress-spinner';

@Component({
  selector: 'app-order-payment-table',
  imports: [MatCellDef, MatHeaderCellDef, MatPaginator, MatRowDef, MatHeaderRowDef, MatProgressSpinner, MatTableModule], 
  templateUrl: './order-payment-table.html'
})
export class OrderPaymentTable {


  protected displayedColumns: string[] = ['id', 'transactionReference', 'subtotal',
    'taxAmount', 'paymentMethod', 'paidAt', 'status'];
  protected pageSize = signal(10);
  protected currentPage = signal(1);
  protected totalItems = signal(0);
  protected isLoading = signal(false);
  private allOrderPayments = signal<OrderPayment[]>([]);
  public pagedPaymentOrders = computed(() => this.allOrderPayments());
  private paymentOrderService = inject(OrderPaymentService);


  ngOnInit(): void {
    this.loadOrders();
  }

  private loadOrders() {
    this.isLoading.set(true);
    this.paymentOrderService.getOrderPayments(this.currentPage(), this.pageSize())
      .subscribe({
        next: (response: PagedOrderPayment) => {
          this.allOrderPayments.set(response.data);
          this.totalItems.set(response.totalCount);
          this.isLoading.set(false);
        },
        error: (err) => {
          console.error('Error al obtener locaciones:', err);
          this.isLoading.set(false);
          this.allOrderPayments.set([]); // Limpiamos en caso de error
        }
      });
  }
  onPageChange(event: PageEvent): void {
    this.currentPage.set(event.pageIndex + 1);
    this.pageSize.set(event.pageSize);
    this.loadOrders();
  }
}
