import { Component, inject, signal } from '@angular/core';
import { MatIcon } from "@angular/material/icon";
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from "@angular/material/dialog";
import { ProductImage } from '../../../../core/interfaces/product-image.interface';
import { MatRadioGroup, MatRadioButton } from "@angular/material/radio";
import { ReactiveFormsModule, ɵInternalFormsSharedModule } from '@angular/forms';
import { MatFormField, MatLabel, MatHint } from "@angular/material/select";
import { MatAutocompleteModule } from "@angular/material/autocomplete";
import { MatInput } from "@angular/material/input";
import { ProductImageService } from '../../../../core/services/product-image.service';
import { take } from 'rxjs';
import { NotificationService } from '../../../../core/services/notification.service';
import { MatProgressSpinner } from "@angular/material/progress-spinner";
import { ProductStore } from '../../../../core/services/product-store';

interface NewImage {
  file: File;
  preview: string;
}

@Component({
  selector: 'app-product-image-dialog',
  imports: [MatIcon, MatDialogModule, ReactiveFormsModule, MatRadioGroup,
    MatRadioButton, ɵInternalFormsSharedModule, MatAutocompleteModule,
    MatFormField, MatLabel, MatHint, MatInput, MatProgressSpinner],
  templateUrl: './product-image-dialog.html',
  styleUrl: './product-image-dialog.css',
}) export class ProductImageDialog {

  constructor() {
    this.loadImages(this.data);
  }

  private dialogRef = inject(MatDialogRef<ProductImageDialog>);
  private data = inject(MAT_DIALOG_DATA);
  protected existingImages = signal<ProductImage[]>([]);
  protected newImage: NewImage | null = null;
  public previewImage: string | null = null;
  protected productImageService = inject(ProductImageService);
  protected isLoading = signal(false);
  private notify = inject(NotificationService);
  protected productStore = inject(ProductStore);

  loadImages(data: any) {
    this.existingImages.set(data?.images || []);
  }

  onFileSelected(event: any) {
    const file = event.target.files[0]; 
    if (!file) return;

    if (this.newImage) {
      URL.revokeObjectURL(this.newImage.preview);
    }

    this.newImage = {
      file,
      preview: URL.createObjectURL(file)
    };

    event.target.value = '';
  }

  removeNew() {
    if (this.newImage) {
      URL.revokeObjectURL(this.newImage.preview);
      this.newImage = null;
    }
  }
  save(providerId: string) {
    if (!this.newImage) return;

    const providers: Record<string, string> = {
      '2': 'External',
      '3': 'Cloudinary'
    };

    this.isLoading.set(true);

    const uploadData = {
      file: this.newImage.file,
      imageUrl: !this.newImage.file ? this.newImage.preview : undefined,
      provider: providers[providerId] || 'Local'
    };

    this.productImageService.uploadProductImage(this.data.productId, uploadData)
      .subscribe({
        next: (response: any) => { // Recibes el objeto completo del API
          this.isLoading.set(false);
          const newlyCreatedImage: ProductImage = response.data;
          this.existingImages.update(prevImages => [...prevImages, newlyCreatedImage]);
          this.removeNew();
          this.notify.show('Se ha agregado una imagen.');
          this.productStore.loadProductsAdmin();
        },
        error: (err) => {
          this.isLoading.set(false);
          this.notify.show('error: ' + err);
        }
      });
  }
  openPreview(url: string) {
    this.previewImage = url;
  }

  closePreview() {
    this.previewImage = null;
  }

  deleteExisting(img: ProductImage) {

    const provider = img.provider || '';

    this.productImageService.deleteProductImage(img.id, provider)
      .pipe(take(1))
      .subscribe({
        next: () => {
          this.existingImages.update(images => images.filter(i => i.id !== img.id)); this.notify.show('Imagen eliminada correctamente.');
          this.productStore.loadProductsAdmin();

        },
        error: (err) => {
          this.notify.show('Error al eliminar la imagen.');
          console.error(err);
        }
      });
  }

  close() {
    this.dialogRef.close();
  }

  updateUrlPreview(url: string) {
    if (!url.trim()) {
      this.newImage = null;
      return;
    }

    if (url.startsWith('http')) {
      this.newImage = {
        file: null as any,
        preview: url
      };
    }
  }

  addUrlImage(url: string) {
    this.updateUrlPreview(url);
  }
}