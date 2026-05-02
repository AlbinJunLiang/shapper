import { Component, computed, inject, input, signal } from '@angular/core';
import { CategoriesResponse } from '../../core/interfaces/category.interface';
import { CategoryStore } from '../../core/services/category-store';
import { Router } from '@angular/router';
import { scrollToTop } from '../../core/shared/utils/scroll-util';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-category-viewer',
  imports: [TranslateModule],
  templateUrl: './category-viewer.html'
})
export class CategoryViewer {

  categories = input.required<CategoriesResponse[]>();
  PAGE_SIZE = input.required<number>();
  private router = inject(Router);

  public categoryStore = inject(CategoryStore);
  actualPage = signal(1);
  canLoadMore = computed(() => {
    return this.categories().length < this.categoryStore.totalCategories();
  });


  imageError: { [key: number]: boolean } = {};

  manageImageError(id: number) {
    this.imageError[id] = true;
  }

  loadMore() {
    if (this.categories().length < this.categoryStore.totalCategories()) {
      this.actualPage.update(prev => prev + 1);
      this.categoryStore.loadCategories(this.actualPage(), this.PAGE_SIZE());
    }
  }


  navigateToCategory(categoryId: string | number) {
    this.router.navigate(['/products', 'filter'], {
      queryParams: {
        subcategoryIds: categoryId, pageSize: 8
      },
    });

    scrollToTop();
  }
}
