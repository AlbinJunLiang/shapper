import { Component, inject, signal } from '@angular/core';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatProgressSpinner } from '@angular/material/progress-spinner';
import { MatHeaderCellDef, MatHeaderRowDef, MatRowDef, MatTableModule, MatColumnDef } from '@angular/material/table';
import { Product } from '../../../core/interfaces/product.interface';
import { MatIcon } from "@angular/material/icon";
import { MatMenuModule } from '@angular/material/menu';
import { Searcher } from "./searcher/searcher";
import { ProductStore } from '../../../core/services/product-store';
import { MatDialog } from '@angular/material/dialog';
import { ProductImageDialog } from './product-image-dialog/product-image-dialog';
import { ProductForm } from "./product-form/product-form";
import { VerifyDialog } from '../../dialog/verify-dialog/verify-dialog';
import { NotificationService } from '../../../core/services/notification.service';
import { FeaturedProductService } from '../../../core/services/featured-product.service';

@Component({
  selector: 'app-product-table',
  imports: [MatPaginator, MatHeaderRowDef, MatRowDef, MatHeaderCellDef,
    MatColumnDef, MatProgressSpinner, MatTableModule, MatIcon, MatMenuModule, Searcher, ProductForm],
  templateUrl: './product-table.html',
  styleUrl: './product-table.css',
})
export class ProductTable {

  protected displayedColumns: string[] = ['actions', 'id', 'name', 'description', 'price', 'taxAmount', 'discount', 'quantity', 'status'];
  protected pageSize = signal(10);
  protected currentPage = signal(1);
  protected productStore = inject(ProductStore);
  protected featuredProductService = inject(FeaturedProductService);

  private readonly dialog = inject(MatDialog);
  protected isFormOpen = signal(false);

  public editableProduct: Product | null = null;
  private notify = inject(NotificationService);


  ngOnInit(): void {
    this.loadProducts();
  }

  private loadProducts() {
    this.productStore.loadProductsAdmin(this.currentPage(), this.pageSize());
  }

  onPageChange(event: PageEvent): void {
    this.currentPage.set(event.pageIndex + 1);
    this.pageSize.set(event.pageSize);
    this.loadProducts();
  }


  openImageManager(product: Product) {
    this.dialog.open(ProductImageDialog, {
      width: '550px',
      maxWidth: '95vw',
      maxHeight: '90vh',
      enterAnimationDuration: '100ms',
      exitAnimationDuration: '100ms',
      panelClass: 'custom-tailwind-dialog',
      backdropClass: 'custom-backdrop',
      data: {
        images: product.images || [],
        productId: product.id
      }
    });
  }


  protected goToCreate() {
    this.isFormOpen.update(v => !v);
    this.editableProduct = null;
  }

  protected goToEdit(product: Product) {
    this.editableProduct = product;
    this.isFormOpen.set(true);
  }



  deleteProduct(product: Product) {
    const dialogRef = this.dialog.open(VerifyDialog, {
      width: '350px',
      data: {
        title: 'Confirmar eliminación',
        message: `¿Estás seguro de que deseas eliminar "${product.name}"? Esta acción no se puede deshacer.`
      }
    });

    dialogRef.afterClosed().subscribe(result => {

      if (result) {
        this.productStore.deleteProduct(product.id).subscribe({
          next: () => {
            // nosotros solo mostramos el mensaje.
            this.notify.show("Eliminado con éxito");
          },
          error: (err) => {
            // Si el hpta backend tira error (ej: tiene subcategorías), lo mostrás aquí
            this.dialog.open(VerifyDialog, {
              data: {
                title: 'Error al eliminar',
                message: err.error?.message || 'No se puede borrar este producto.'
              }
            });
          }
        });
      }
    });
  }

  // En tu componente de administración o lista de productos
  markAsFeatured(product: Product) {
    this.featuredProductService.featureProduct(product.id).subscribe({
      next: (res) => {
        // Usamos el mensaje que viene del servidor (ej: "Producto destacado con éxito")
        this.notify.show("Agregado como destacado.");
      },
      error: (err) => {
        this.notify.show("Ha ocurrido un error");
      }
    });
  }

}
