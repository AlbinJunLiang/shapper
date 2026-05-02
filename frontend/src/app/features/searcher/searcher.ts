import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { MatIconModule } from "@angular/material/icon";
import { MatButtonModule } from "@angular/material/button";
import { MatAutocompleteModule, MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { ProductDialogService } from '../../core/services/product-dialog.service';
import { TranslateModule } from '@ngx-translate/core';
import { Product } from '../../core/interfaces/product.interface';
import { ProductStore } from '../../core/services/product-store';

@Component({
  selector: 'app-searcher',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatIconModule,
    MatButtonModule,
    MatAutocompleteModule,
    TranslateModule
  ],
  templateUrl: './searcher.html',
  styleUrl: './searcher.css',
})
export class Searcher implements OnInit {
  protected searchControl = new FormControl<string | Product>('');
  private productDialogService = inject(ProductDialogService);
  private productStore = inject(ProductStore);
  public products = this.productStore.products;
  public searchTerm = signal('');
  public hasSearched = signal(false);

  public filteredProducts = computed(() => {
    const term = this.searchTerm().toLowerCase();

    return this.products().filter((p: Product) =>
      p.name.toLowerCase().includes(term) ||
      p.description.toLowerCase().includes(term)
    ).slice(0, 10);
  });
  ngOnInit() {
    this.searchControl.valueChanges.pipe(
      debounceTime(200),
      distinctUntilChanged()
    ).subscribe(value => {
      const term = typeof value === 'string' ? value : value?.name;
      this.searchTerm.set(term || '');

      this.hasSearched.set(false);
    });
  }


  async performSearch() {
    const value = this.searchControl.value;
    const term = typeof value === 'string' ? value : value?.name;

    if (term) {
      this.hasSearched.set(true);
      this.productStore.loadSearchProducts(term, 10);
    }
  }


  displayFn(product: Product): string {
    return product && product.name ? product.name : '';
  }

  onProductSelected(event: MatAutocompleteSelectedEvent) {
    const product = event.option.value as Product;
    if (product) {
      this.productDialogService.openDetail(product);
    }
  }
}