import { computed, effect, Injectable, signal } from '@angular/core';
import { environment } from '../../../environments/environment.development';
import { ProductAdded } from '../interfaces/product-added.interface';

@Injectable({
  providedIn: 'root',
})
export class CartStorage {

  private readonly STORAGE_KEY = environment.cartStorageKey;
  private _cartItems = signal<ProductAdded[]>(this.loadFromStorage());
  public cartItems = this._cartItems.asReadonly();


  constructor() {
    effect(() => {
      sessionStorage.setItem(this.STORAGE_KEY, JSON.stringify(this._cartItems()));
    });
  }

  private loadFromStorage(): ProductAdded[] {
    if (typeof window !== 'undefined' && window.sessionStorage) {
      const saved = sessionStorage.getItem(this.STORAGE_KEY);
      return saved ? JSON.parse(saved) : [];
    }
    return [];
  }

  public totalAmount = computed(() => {
    return this._cartItems().reduce((acc, item) =>
      acc + (item.currentPrice * Number(item.quantity)), 0
    );
  });

  public totalUnits = computed(() => {
    return this._cartItems().reduce((acc, item) =>
      acc + Number(item.quantity), 0
    );
  });

  addProduct(product: ProductAdded) {
    this._cartItems.update(items => {
      const index = items.findIndex(i => i.idProduct === product.idProduct);

      if (index !== -1) {
        const updatedItems = [...items];
        const currentQty = Number(updatedItems[index].quantity);
        const addedQty = (product.quantity);

        updatedItems[index] = {
          ...updatedItems[index],
          imageUrl: product.imageUrl,
          quantity: (currentQty + addedQty)
        };
        return updatedItems;
      }
      return [...items, product];
    });
  }

  updateProductById(id: number, updatedData: Partial<ProductAdded>) {
    this._cartItems.update(items =>
      items.map(item =>
        item.idProduct === id
          ? { ...item, ...updatedData } 
          : item 
      )
    );
  }

  removeProduct(id: number) {
    this._cartItems.update(items =>
      items.filter(item => item.idProduct !== id)
    );
  }

  clearCart() {
    this._cartItems.set([]);
    sessionStorage.removeItem(this.STORAGE_KEY);
  }
}
