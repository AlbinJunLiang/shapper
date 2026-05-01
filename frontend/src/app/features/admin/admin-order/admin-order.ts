import { Component, inject, Injector, signal, viewChild } from '@angular/core';
import { MatProgressSpinner } from "@angular/material/progress-spinner";
import { MatPaginator, PageEvent } from "@angular/material/paginator";
import { OrderService } from '../../../core/services/order-service';
import { UserStore } from '../../../core/services/user-store';
import { OrderTable } from '../../../core/interfaces/order-table.interface';
import { MatTableDataSource, MatHeaderCellDef, MatCellDef, MatTableModule } from '@angular/material/table';
import { Subscription, take } from 'rxjs';
import { getStatusTranslationKey } from '../../../core/shared/utils/order-status';
import { toObservable } from '@angular/core/rxjs-interop';
import { TranslateModule } from '@ngx-translate/core';
import { DatePipe } from '@angular/common';
import { Router } from '@angular/router';


@Component({
  selector: 'app-admin-order',
  imports: [MatProgressSpinner, MatPaginator, MatHeaderCellDef, MatCellDef,
    MatTableModule, TranslateModule, DatePipe],
  templateUrl: './admin-order.html',
  styleUrl: './admin-order.css',
})
export class AdminOrder {
  private readonly orderService = inject(OrderService);
  private readonly userStore = inject(UserStore);
  private readonly injector = inject(Injector);
  protected totalItems = signal(0);
  protected pageSize = signal(10);
  protected currentPage = signal(1);
  protected isLoading = signal(false);
  protected displayedColumns: string[] = ['reference', 'date', 'status', 'total'];
  protected dataSource = new MatTableDataSource<OrderTable>([]);
  protected orderStatus = getStatusTranslationKey;
  readonly paginator = viewChild(MatPaginator);
  private userSubscription?: Subscription;
  private router = inject(Router);



  ngOnInit(): void {
    this.userSubscription = toObservable(this.userStore.userSyncData, { injector: this.injector })
      .pipe(
        take(1)
      )
      .subscribe(user => {
        this.loadOrders();
      });
  }

  refresh() {
    this.loadOrders();
  }

  /**
   * Método que se dispara cada vez que el usuario interactúa con el paginador
   */
  onPageChange(event: PageEvent): void {
    this.currentPage.set(event.pageIndex + 1); // MatPaginator es base 0, API suele ser base 1
    this.pageSize.set(event.pageSize);
    this.loadOrders();

  }

  private loadOrders(): void {
    this.isLoading.set(true);

    // Llamada al servicio con parámetros de paginación
    this.orderService.getOrders(this.currentPage(), this.pageSize())
      .subscribe({
        next: (response) => {
          this.handleResponse(response.data, response.totalCount || 0);
        },
        error: (err) => {
          console.error('Error:', err);
          this.isLoading.set(false);
        }
      });
  }

  private handleResponse(data: any[], total: number): void {
    const orders: OrderTable[] = data.map(o => ({
      reference: o.orderReference || 'N/A',
      date: o.createdAt,
      status: o.status || 'COMPLETED',
      total: o.total
    }));

    this.dataSource.data = orders;
    this.totalItems.set(total);
    this.isLoading.set(false);
  }

  openDetails(order: OrderTable) {
    this.router.navigate(['admin/orders', order.reference]);
  }

  ngOnDestroy(): void {
    this.userSubscription?.unsubscribe();
  }
}
