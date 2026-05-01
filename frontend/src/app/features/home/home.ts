import { Component, inject } from '@angular/core';
import { Carousel } from '../carousel/carousel';
import { CategoryViewer } from '../category-viewer/category-viewer';
import { PaymentMethodInfo } from '../payment-method-info/payment-method-info';
import { CategoryStore } from '../../core/services/category-store';
import { MatProgressSpinner } from "@angular/material/progress-spinner";



@Component({
  selector: 'app-home',
  imports: [Carousel, CategoryViewer, PaymentMethodInfo, MatProgressSpinner],
  templateUrl: './home.html',
  styleUrl: './home.css',
})
export class Home {

  public categoryStore = inject(CategoryStore);
  public categories = this.categoryStore.categories;
  readonly PAGE_SIZE = 8;
  

  ngOnInit() {
    if (this.categories().length === 0) {
      this.categoryStore.loadCategories(1,this.PAGE_SIZE);
    }
  }
}
