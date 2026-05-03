import { Component, inject, signal } from '@angular/core';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatProgressSpinner } from '@angular/material/progress-spinner';
import { MatTableModule } from '@angular/material/table';
import { CategoriesResponse } from '../../../core/interfaces/category.interface';
import { MatMenuModule } from "@angular/material/menu";
import { MatIcon } from "@angular/material/icon";
import { ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from "@angular/material/button";
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from "@angular/material/input";
import { CategoryForm } from "./category-form/category-form";
import { CategoryStore } from '../../../core/services/category-store';
import { NotificationService } from '../../../core/services/notification.service';
import { MatDialog } from '@angular/material/dialog';
import { VerifyDialog } from '../../dialog/verify-dialog/verify-dialog';

@Component({
  selector: 'app-category-table',
  imports: [MatTableModule,
    MatPaginator,
    MatProgressSpinner,
    MatMenuModule,
    MatIcon,
    MatButtonModule, // Importante para MatAnchor y MatIconButton
    MatFormFieldModule, // Provee la estructura del field
    MatInputModule, // ¡ESTE ES VITAL! Registra cómo se ve el input dentro del field
    ReactiveFormsModule, CategoryForm],
  templateUrl: './category-table.html',
  styleUrl: './category-table.css',
})
export class CategoryTable {

  protected displayedColumns: string[] = ['actions', 'id', 'name', 'description', 'imageUrl'];
  protected pageSize = signal(8);
  protected currentPage = signal(1);
  protected categoryStore = inject(CategoryStore);
  protected isFormOpen = signal(false);
  public editableCategory: CategoriesResponse | null = null;
  private notify = inject(NotificationService);
  private dialog = inject(MatDialog);


  ngOnInit(): void {
    this.loadCategories();
  }

  private loadCategories() {
    this.categoryStore.getCategories(this.currentPage(), this.pageSize());
  }


  goToEdit(category: CategoriesResponse) {
    this.editableCategory = category;
    this.isFormOpen.set(true);
  }

  goToCreate() {
    this.isFormOpen.update(v => !v);
    this.editableCategory = null;
  }


  deleteCategory(category: CategoriesResponse) {
    const dialogRef = this.dialog.open(VerifyDialog, {
      width: '350px',
      data: {
        title: 'Confirmar eliminación',
        message: `¿Estás seguro de que deseas eliminar "${category.name}"? Esta acción no se puede deshacer.`
      }
    });

    dialogRef.afterClosed().subscribe(result => {

      if (result) {
        this.categoryStore.deleteCategory(category.id).subscribe({
          next: () => {
            // nosotros solo mostramos el mensaje.
            this.notify.show("Eliminado con éxito");
          },
          error: (err) => {
            // Si el hpta backend tira error (ej: tiene subcategorías), lo mostrás aquí
            this.dialog.open(VerifyDialog, {
              data: {
                title: 'Error al eliminar',
                message: err.error?.message || 'No se puede borrar esta categoría.'
              }
            });
          }
        });
      }
    });
  }

  onPageChange(event: PageEvent): void {
    this.currentPage.set(event.pageIndex + 1);
    this.pageSize.set(event.pageSize);
    this.loadCategories();
  }
}
