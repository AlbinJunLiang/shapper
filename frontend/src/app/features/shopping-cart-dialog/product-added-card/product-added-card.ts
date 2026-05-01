import { Component, inject, signal, input } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatIcon } from '@angular/material/icon';
import { CartStorage } from '../../../core/services/cart-storage';
import { CommonModule } from '@angular/common';
import { ProductAdded } from '../../../core/interfaces/product-added.interface';
import { TranslateModule } from '@ngx-translate/core';
import { environment } from '../../../../environments/environment.development';

@Component({
  selector: 'app-product-added-card',
  imports: [FormsModule, MatIcon, CommonModule, TranslateModule],
  templateUrl: './product-added-card.html',
  styleUrl: './product-added-card.css',
})
export class ProductAddedCard {

  protected currency = environment.currency;
  public productAdded = input.required<ProductAdded>();
  public cartStorage = inject(CartStorage);
  protected quantity = signal(1);
  protected showDetails = signal(false);
  protected extraDetails = signal('');

  remove(id: number) {
    this.cartStorage.removeProduct(id);
  }

  acceptDetails(quantity: number, additionalDetails: string) {
    this.setProductAdded(quantity, additionalDetails);
    this.showDetails.set(false);
  }

  setProductAdded(quantity: number, additionalDetails: string) {
    if (quantity < 1000000) {
      this.quantity.set(quantity);
    }

    const product = this.productAdded();
    product.quantity = this.quantity();
    product.additionalDetails = additionalDetails;
    this.cartStorage.updateProductById(product.idProduct, product);
  }
}