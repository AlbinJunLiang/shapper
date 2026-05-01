import { Component, ElementRef, inject, signal, viewChild, Injector, afterNextRender } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatIcon } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { CartStorage } from '../../core/services/cart-storage';
import { Product } from '../../core/interfaces/product.interface';
import { ProductReview } from "./product-review/product-review";
import { UserStore } from '../../core/services/user-store';
import { ReviewStore } from '../../core/services/review-store';
import { REVIEW_CONFIG } from './review-config';
import { ProductStatus } from '../../core/enums/product-status.enum';
import { environment } from '../../../environments/environment.development';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-product-dialog',
  standalone: true,
  imports: [CommonModule, MatDialogModule, MatDialogModule, MatIcon, MatChipsModule, ProductReview, TranslateModule],
  styleUrl: './product-dialog.css',

  templateUrl: './product-dialog.html',
})

export class ProductDialog {
  protected currency = environment.currency;
  protected gallery = viewChild<ElementRef<HTMLDivElement>>('gallery');
  protected modalContainer = viewChild<ElementRef<HTMLDivElement>>('modalContainer');
  protected quantity = 1;
  protected isAccordionOpen = signal(false);
  protected isAccordionOpen2 = signal(false);

  public data = inject<Product>(MAT_DIALOG_DATA);
  private dialogRef = inject(MatDialogRef<ProductDialog>);
  public parsedDetails = signal<any>({});
  public cartStorage = inject(CartStorage);
  protected reviewStore = inject(ReviewStore);
  protected userStore = inject(UserStore);
  protected Status = ProductStatus;
  private injector = inject(Injector);
  protected selectedImage = '';
  protected transformOrigin = 'center';




  ngOnInit() {
    this.selectedImage = this.data.images?.length > 0
      ? this.data.images[0].imageUrl
      : 'assets/shapper.png';

    try {
      const details = typeof this.data.details === 'string'
        ? JSON.parse(this.data.details)
        : this.data.details;
      this.parsedDetails.set(details || {});
    } catch (e) {
      this.parsedDetails.set({});
    }
  }

  updateImage(img: string) { this.selectedImage = img; }

  changeQty(val: number) {
    if (this.quantity + val >= 1) this.quantity += val;
  }

  // Función para mover la galería horizontalmente
  scrollGallery(offset: number) {
    this.gallery()?.nativeElement.scrollBy({ left: offset, behavior: 'smooth' });
  }
  toggleReviews() {
    this.isAccordionOpen2.update(v => !v);

    if (this.isAccordionOpen2()) {
      this.reviewStore.loadReviews(Number(this.data.id), REVIEW_CONFIG.PAGE_INITIAL, REVIEW_CONFIG.PAGE_SIZE);
      afterNextRender(() => {

        const container = this.modalContainer()?.nativeElement;
        if (container) {
          container.scrollTo({
            top: container.scrollHeight,
            behavior: 'smooth'
          });
        }
      }, { injector: this.injector });
    }
  }

  addProduct() {
    const imgUrl = this.selectedImage || 'assets/shapper.png';
    this.cartStorage.addProduct({
      idProduct: this.data.id,
      name: this.data.name,
      price: this.data.price,
      currentPrice: this.data.newPrice,
      quantity: this.quantity,
      additionalDetails: '',
      imageUrl: imgUrl
    });
    this.dialogRef.close({ success: true });
  }


  onMouseMove(e: MouseEvent) {
    const { left, top, width, height } = (e.currentTarget as HTMLElement).getBoundingClientRect();
    // Calculamos el porcentaje de la posición del mouse dentro de la imagen
    const x = ((e.clientX - left) / width) * 100;
    const y = ((e.clientY - top) / height) * 100;
    this.transformOrigin = `${x}% ${y}%`;
  }
}