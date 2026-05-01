import { Component, OnInit, inject } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ProductDialogService } from '../../core/services/product-dialog.service';
import { ProductService } from '../../core/services/product-service';
import { Product } from '../../core/interfaces/product.interface';

@Component({
  selector: 'app-details-dialog-wrapper',
  standalone: true, // Estándar moderno
  template: '',    // No necesitas archivo HTML para un wrapper de lógica
})
export class DetailsDialogWrapper implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private productDialogService = inject(ProductDialogService);
  private productService = inject(ProductService);



  ngOnInit() {
    this.openDialog();
  }

  openDialog() {
    // 1. Obtener ID de la URL y convertirlo a número
    const idFromUrl = Number(this.route.snapshot.params['id']);

    // 2. Usar ese ID para la petición (si no existe, podrías usar el 11 de prueba)
    const idToFetch = idFromUrl || 11;

    this.productService.getProduct(idToFetch).subscribe({
      next: (data: Product) => {
        this.productDialogService.openDetail({
          id: data.id, // Usa el ID real que viene del servidor
          name: data.name,
          description: data.description || 'Sin descripción', // Evita textos fijos
          price: data.price,
          newPrice: data.newPrice,
          taxAmount: data.taxAmount,
          discount: data.discount,
          details: data.details,
          quantity: data.quantity,
          status: data.status,
          subcategoryName: data.subcategoryName, // Asegúrate que el tipo coincida
          images: data.images
        })
          .afterClosed()
          .subscribe(() => {
            this.router.navigate(['/products']);
          });
      },
      error: (err: any) => {
        console.error("Error al cargar producto:", err);
        // Opcional: mostrar un mensaje al usuario si el producto no existe
      }
    });
  }
}