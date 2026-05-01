import { Component, effect, inject, input, model, signal } from '@angular/core';
import { MatFormField, MatSuffix, MatError } from "@angular/material/select";
import { MatInput } from "@angular/material/input";
import { MatIcon } from "@angular/material/icon";
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatAnchor, MatButtonModule, MatIconButton } from "@angular/material/button";
import { MatProgressSpinnerModule } from "@angular/material/progress-spinner";
import { NotificationService } from '../../../../core/services/notification.service';
import { VerifyDialog } from '../../../dialog/verify-dialog/verify-dialog';
import { MatDialog } from '@angular/material/dialog';
import { CategoriesResponse } from '../../../../core/interfaces/category.interface';
import { CategoryStore } from '../../../../core/services/category-store';
import { MatRadioGroup, MatRadioButton } from '@angular/material/radio';


@Component({
  selector: 'app-category-form',
  imports: [MatFormField, MatInput, MatIcon, MatSuffix, ReactiveFormsModule,
    MatAnchor, MatIconButton, MatButtonModule, MatError, MatProgressSpinnerModule,
    MatRadioGroup, MatRadioButton],
  templateUrl: './category-form.html',
  styleUrl: './category-form.css',
})
export class CategoryForm {


  public isFormOpen = model<boolean | null>(null);
  protected isUrl = signal(false);
  protected imagePreview = signal<string | ArrayBuffer | null>(null);
  protected categoryStore = inject(CategoryStore);
  private notify = inject(NotificationService);
  private dialog = inject(MatDialog);
  public editableCategory = input<CategoriesResponse | null>(null);

  selectedFile!: File;
  private fb = inject(NonNullableFormBuilder);

  categoryForm = this.fb.group({
    name: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(100)]],
    description: ['', [Validators.maxLength(300)]],
    imageUrl: ['', [Validators.maxLength(800)]],
    image: [null],
    imageProvider: ['']
  });

  constructor() {
    effect(() => {
      const editableCategory = this.editableCategory();
      if (editableCategory) {
        this.imagePreview.set(editableCategory.imageUrl);
        this.categoryForm.patchValue({
          imageUrl: editableCategory.imageUrl,
          name: editableCategory.name,
          description: editableCategory.description,
          imageProvider: editableCategory.imageProvider?.toLocaleLowerCase()
        });
      }
    })


  }// 1. Cambia la declaración

  onFileSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    this.isUrl.set(false);

    if (input.files?.length) {
      this.selectedFile = input.files[0];
      const reader = new FileReader();

      reader.onload = () => {
        // 2. Usa .set() para actualizar
        this.imagePreview.set(reader.result);
        // Ya no necesitas this.cd.detectChanges();
      };

      reader.readAsDataURL(this.selectedFile);
    }
  }

  removeImage(fileInput?: HTMLInputElement) {
    this.imagePreview.set(null); // 3. Limpiar con .set()
    this.selectedFile = null as any;
    this.categoryForm.patchValue({ image: null });
    this.categoryForm.get('imageProvider')?.reset();

    if (fileInput) {
      fileInput.value = '';
    }
  }

  protected resetUrlInput(input: HTMLInputElement) {
    input.value = '';
    this.imagePreview.set(null); // 4. Limpiar con .set()
    this.isUrl.set(false);
    this.categoryForm.patchValue({
      imageUrl: ''
    });
  }

  createCategory() {
    if (this.categoryForm.invalid) return;

    const formValues = this.categoryForm.getRawValue();

    const editableCategory = this.editableCategory();
    if (editableCategory) {

      this.categoryStore.updateCategory(editableCategory?.id, formValues, this.selectedFile)
        .subscribe({
          next: (res) => {
            // ÉXITO: Mensaje y cerrar
            this.notify.show("¡Categoría actualizada!");
            this.isFormOpen.set(false);
          },
          error: (err) => {
            console.log(err)
            // ERROR: Aquí capturás el hpta error de SQL (el del ImageUrl NULL)
            this.dialog.open(VerifyDialog, {
              width: '350px',
              data: {
                title: 'Error de base de datos',
                message: err.error?.message || 'Revisá los campos.'
              }
            });
          }
        });

    } else {

      this.categoryStore.createCategory(formValues, this.selectedFile)
        .subscribe({
          next: (res) => {
            // ÉXITO: Mensaje y cerrar
            this.notify.show("¡Categoría creada!");
            this.isFormOpen.set(false);
          },
          error: (err) => {
            console.log(err)
            // ERROR: Aquí capturás el hpta error de SQL (el del ImageUrl NULL)
            this.dialog.open(VerifyDialog, {
              width: '350px',
              data: {
                title: 'Error de base de datos',
                message: err.error?.message || 'Revisá los campos.'
              }
            });
          }
        });
    }
  }
  onCancel() {
    this.isFormOpen.set(false);
  }
}
