import { Component, inject } from '@angular/core';
import { Carousel } from '../carousel/carousel';
import { CategoryViewer } from '../category-viewer/category-viewer';
import { PaymentMethodInfo } from '../payment-method-info/payment-method-info';
import { CategoryStore } from '../../core/services/category-store';
import { HomeStore } from '../../core/services/home-store';

@Component({
  selector: 'app-home',
  imports: [Carousel, CategoryViewer, PaymentMethodInfo],
  templateUrl: './home.html',
  styleUrl: './home.css',
})
export class Home {

  protected homeStore = inject(HomeStore);
  public categoryStore = inject(CategoryStore);
  public categories = this.categoryStore.categories;
  readonly PAGE_SIZE = 8;

}
