import { Component, inject, signal, OnInit, OnDestroy, Injector, viewChild } from '@angular/core';
import { DatePipe } from '@angular/common';
import { toObservable } from '@angular/core/rxjs-interop';
import { MatPaginator, MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { filter, take, Subscription } from 'rxjs';
import { OrderService } from '../../core/services/order-service';
import { UserStore } from '../../core/services/user-store';
import { MatProgressSpinner } from "@angular/material/progress-spinner";
import { MatDialog } from '@angular/material/dialog';
import { OrderDetailDialog } from './order-detail-dialog/order-detail-dialog';
import { getStatusTranslationKey } from '../../core/shared/utils/order-status';
import { TranslateModule } from '@ngx-translate/core';
import { OrderTable } from '../../core/interfaces/order-table.interface';
import { environment } from '../../../environments/environment.development';


@Component({
  selector: 'app-order',
  standalone: true,
  imports: [MatTableModule, MatPaginatorModule, DatePipe, MatProgressSpinner, TranslateModule],
  templateUrl: './order.html',
  styleUrl: './order.css',
})
export class Order implements OnInit, OnDestroy {

  protected currency = environment.currency;
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
  private dialog = inject(MatDialog);



  ngOnInit(): void {
    this.userSubscription = toObservable(this.userStore.userSyncData, { injector: this.injector })
      .pipe(
        filter(user => !!user?.data?.id),
        take(1)
      )
      .subscribe(user => {
        this.loadOrders(user!.data.id);
      });
  }

  refresh() {
    const userId = this.userStore.userSyncData()?.data?.id;

    if (userId) {
      this.loadOrders(userId);
    } else {
      console.warn('No se puede refrescar: Usuario no identificado');
    }
  }

  /**
   * Método que se dispara cada vez que el usuario interactúa con el paginador
   */
  onPageChange(event: PageEvent): void {
    this.currentPage.set(event.pageIndex + 1); // MatPaginator es base 0, API suele ser base 1
    this.pageSize.set(event.pageSize);

    const userId = this.userStore.userSyncData()?.data.id;
    if (userId) {
      this.loadOrders(userId);
    }
  }

  private loadOrders(userId: number): void {
    this.isLoading.set(true);

    this.orderService.getOrderByUserId(this.currentPage(), this.pageSize(), userId)
      .subscribe({
        next: (response) => {
          // Asumiendo que tu API devuelve { data: Pedido[], totalCount: number }
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

    console.log(data)
    this.dataSource.data = orders;
    this.totalItems.set(total);
    this.isLoading.set(false);
  }

  openDetails(order: OrderTable) {

    this.dialog.open(OrderDetailDialog, {
      data: { reference: order.reference },
      width: '500px',
      autoFocus: false
    });
  }

  ngOnDestroy(): void {
    this.userSubscription?.unsubscribe();
  }
}