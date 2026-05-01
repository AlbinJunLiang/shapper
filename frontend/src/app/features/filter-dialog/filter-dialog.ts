import { Component, inject } from '@angular/core';
import { MatDialogClose, MatDialogRef } from "@angular/material/dialog";
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatSliderModule } from '@angular/material/slider';
import { Router } from '@angular/router';
import { FilterCategoryResponse } from '../../core/interfaces/filter-category-response.interface';
import { scrollToTop } from '../../core/shared/utils/scroll-util';
import { FilterStore } from '../../core/services/filter-storage';
import { MatIcon } from "@angular/material/icon";
import { TranslateModule } from '@ngx-translate/core';
import { environment } from '../../../environments/environment.development';

@Component({
  selector: 'app-filter-dialog',
  imports: [MatDialogClose, MatCheckboxModule, MatSliderModule, MatIcon, TranslateModule],
  templateUrl: './filter-dialog.html',
  styleUrl: './filter-dialog.css',
})
export class FilterDialog {
  protected currency = environment.currency;
  public dialogRef = inject(MatDialogRef<FilterDialog>);
  public filterStore = inject(FilterStore);
  public data = this.filterStore.categories() as FilterCategoryResponse;
  readonly minLimit = this.data.globalMinPrice;
  readonly maxLimit = this.data.globalMaxPrice * 2;
  private router = inject(Router);
  categories = this.data.categories;
  minPrice = this.filterStore.selectedMin;
  maxPrice = this.filterStore.selectedMax;


  updateParent(completed: boolean, catIndex: number) {
    const cat = this.categories[catIndex];

    if (cat) {
      cat.completed = completed;

      cat.subcategories?.forEach(sub => sub.completed = completed);
      this.categories = [...this.categories];
    }
  }

  updateChild(completed: boolean, catIndex: number, subIndex: number) {
    const cat = this.categories[catIndex];

    if (cat && cat.subcategories) {
      cat.subcategories[subIndex].completed = completed;
      cat.completed = cat.subcategories?.every(s => s.completed);
      this.categories = [...this.categories];
    }
  }

  isIndeterminate(catIndex: number): boolean {
    const cat = this.categories[catIndex];
    if (!cat || !cat.subcategories || cat.subcategories.length === 0) {
      return false;
    }
    const completedCount = cat.subcategories.filter(s => s.completed).length;
    const totalCount = cat.subcategories.length;
    return completedCount > 0 && completedCount < totalCount;
  }

  updateMin(event: any) {
    this.minPrice.set(Number(event.target.value));
  }

  updateMax(event: any) {
    this.maxPrice.set(Number(event.target.value));
  }

  private getSelectedCategories() {
    const result = this.categories.flatMap(cat =>
      (cat.subcategories ?? [])
        .filter(sub => sub.completed)
        .map(sub => sub.id)
    );
    return result;
  }
  filter() {
    this.router.navigate(['/products', 'filter'], {
      queryParams: {
        subcategoryIds: this.getSelectedCategories(),
        minPrice: this.minPrice(),
        maxPrice: this.maxPrice(),
        pageSize: 8
      }
    }).then((navigated) => {
      if (navigated) {
        scrollToTop();
      }
    });

    this.dialogRef.close();
  }
}