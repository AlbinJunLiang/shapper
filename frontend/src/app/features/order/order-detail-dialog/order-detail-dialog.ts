import { Component, DestroyRef, inject, signal } from '@angular/core';
import { CommonModule, CurrencyPipe, DatePipe } from '@angular/common';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatDividerModule } from '@angular/material/divider';
import generatePDF from '../../../core/shared/utils/generate-invoice';
import { OrderService } from '../../../core/services/order-service';
import { OrderResponse } from '../../../core/interfaces/order-data.interface';
import { MatProgressSpinner } from "@angular/material/progress-spinner";
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { getStatusTranslationKey } from '../../../core/shared/utils/order-status';
import { TranslateModule } from '@ngx-translate/core';
import { environment } from '../../../../environments/environment.development';
import { StoreService } from '../../../core/services/store-service';
import { LinkType } from '../../../core/enums/link-type.enum';
import { LinkName } from '../../../core/enums/link-name.enum';

@Component({
  selector: 'app-order-detail-dialog',
  standalone: true,
  imports: [
    CommonModule,
    MatDialogModule,
    MatButtonModule,
    MatDividerModule,
    CurrencyPipe,
    DatePipe,
    MatProgressSpinner,
    TranslateModule
  ],
  templateUrl: './order-detail-dialog.html',
  styleUrl: './order-detail-dialog.css',
})
export class OrderDetailDialog {

  protected currency = environment.currency;
  public data = inject(MAT_DIALOG_DATA);
  private dialogRef = inject(MatDialogRef<OrderDetailDialog>);
  private readonly orderService = inject(OrderService);
  private storeService = inject(StoreService);
  public order = signal<OrderResponse | undefined | null>(undefined);
  protected orderStatus = getStatusTranslationKey;
  private destroyRef = inject(DestroyRef);


  close(): void {
    this.dialogRef.close();
  }

  ngOnInit(): void {
    this.orderService.getOrderByReference(this.data.reference)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (data) => this.order.set(data),
        error: (err) => {
          this.order.set(null);
        }
      });
  }

onGeneratePDF() {
  const result = this.storeService.getLink(LinkType.OTHER, LinkName.STORE_LINK);
  const link = result?.url ?? 'google.com';

  const currentOrder = this.order();
  if (!currentOrder) return;
  generatePDF(currentOrder, link);
}
}