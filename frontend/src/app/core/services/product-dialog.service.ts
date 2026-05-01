import { inject, Injectable } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ProductDialog } from '../../features/product-dialog/product-dialog';
import { Product } from '../interfaces/product.interface';

@Injectable({
  providedIn: 'root'
})
export class ProductDialogService {
  private dialog = inject(MatDialog);

  openDetail(product: Product) {
    return this.dialog.open(ProductDialog, {
      data: product,
      maxWidth: 'none',
      height: 'auto',
      panelClass: 'custom-tailwind-dialog',
      backdropClass: 'custom-backdrop',
      autoFocus: false,
      restoreFocus: false
    });
  }
}