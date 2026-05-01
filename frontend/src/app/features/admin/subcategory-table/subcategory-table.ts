import { Component, computed, inject, signal } from '@angular/core';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatProgressSpinner } from '@angular/material/progress-spinner';
import { MatCellDef, MatHeaderCellDef, MatHeaderRowDef, MatRowDef, MatTableModule } from '@angular/material/table';
import { SubcategoryStore } from '../../../core/services/subcategory-store';
import { NotificationService } from '../../../core/services/notification.service';
import { MatDialog } from '@angular/material/dialog';
import { MatIcon } from "@angular/material/icon";
import { MatMenu, MatMenuModule } from "@angular/material/menu";
import { CategoryStore } from '../../../core/services/category-store';
import { Subcategory } from '../../../core/interfaces/subcategory.interface';
import { VerifyDialog } from '../../dialog/verify-dialog/verify-dialog';
import { SubcategoryForm } from "./subcategory-form/subcategory-form";

@Component({
  selector: 'app-subcategory-table',
  imports: [MatCellDef, MatHeaderCellDef, MatPaginator, MatRowDef,
     MatHeaderRowDef, MatProgressSpinner, MatTableModule, MatIcon,
     MatMenu, MatMenuModule, SubcategoryForm],
  templateUrl: './subcategory-table.html',
  styleUrl: './subcategory-table.css',
})
export class SubcategoryTable {

  protected displayedColumns: string[] = ['actions', 'id', 'name', 'description', 'imageUrl', 'categoryId'];
  protected pageSize = signal(10);
  protected currentPage = signal(1);
  protected subcategoryStore = inject(SubcategoryStore);
  protected categoryStore = inject(CategoryStore);
  protected isFormOpen = signal(false);
  private notify = inject(NotificationService);
  private dialog = inject(MatDialog);
  public editableSubcategory: Subcategory | null = null;

  protected categoryMap = computed(() => {
    const categories = this.categoryStore.categories(); // Asumiendo que tu store tiene un signal categories()
    return categories.reduce((map, cat) => {
      map[cat.id] = cat.name;
      return map;
    }, {} as Record<number, string>);
  });

  protected getCategoryName(id: number): string {
    return this.categoryMap()[id] || '?';
  }

  ngOnInit(): void {
    this.loadSubcategories();
    this.loadCategories();
  }


  deleteSubcategory(subcategory: Subcategory) {
    const dialogRef = this.dialog.open(VerifyDialog, {
      width: '350px',
      data: {
        title: 'Confirmar eliminación',
        message: `¿Estás seguro de que deseas eliminar "${subcategory.name}"? Esta acción no se puede deshacer.`
      }
    });

    dialogRef.afterClosed().subscribe(result => {

      if (result) {
        this.subcategoryStore.deleteSubcategory(subcategory.id).subscribe({
          next: () => {
            // nosotros solo mostramos el mensaje.
            this.notify.show("Eliminado con éxito");
          },
          error: (err) => {
            // Si el hpta backend tira error (ej: tiene subcategorías), lo mostrás aquí
            this.dialog.open(VerifyDialog, {
              data: {
                title: 'Error al eliminar',
                message: err.error?.message || 'No se puede borrar esta subcategoría.'
              }
            });
          }
        });
      }
    });
  }

  protected goToEdit(subcategory: Subcategory) {
    this.editableSubcategory = subcategory;
    this.isFormOpen.set(true);
  }

  protected goToCreate() {
    this.isFormOpen.update(v => !v);
    this.editableSubcategory = null;
  }

  private loadSubcategories() {
    this.subcategoryStore.getSubcategories(this.currentPage(), this.pageSize());
  }

  private loadCategories() {
    this.categoryStore.getCategories(1, 100);
  }

  onPageChange(event: PageEvent): void {
    this.currentPage.set(event.pageIndex + 1);
    this.pageSize.set(event.pageSize);
    this.loadSubcategories();
  }
}