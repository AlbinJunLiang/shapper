import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common'; // Para directivas básicas
import { ReactiveFormsModule, FormControl } from '@angular/forms';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { ProductStore } from '../../../../core/services/product-store';

@Component({
  selector: 'app-searcher',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatIconModule,
    MatButtonModule
  ],
  templateUrl: './searcher.html',
  styleUrl: './searcher.css',
})
export class Searcher {
  protected productStore = inject(ProductStore);
  protected searchControl = new FormControl('');

  async performSearch() {
    const term = this.searchControl.value;
    if (term && term.trim()) {
      this.productStore.setSearchProducts(term, 10);
    }
  }

  clearSearch() {
    this.searchControl.setValue('');
    this.productStore.loadProducts();
  }
}