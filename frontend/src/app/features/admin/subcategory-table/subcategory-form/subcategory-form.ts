import { Component, computed, effect, inject, input, model, signal } from '@angular/core';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { NotificationService } from '../../../../core/services/notification.service';
import { Subcategory } from '../../../../core/interfaces/subcategory.interface';
import { MatDialog } from '@angular/material/dialog';
import { MatError, MatFormField, MatSuffix, MatLabel } from '@angular/material/form-field';
import { SubcategoryStore } from '../../../../core/services/subcategory-store';
import { MatAnchor, MatButtonModule, MatIconButton } from "@angular/material/button";
import { MatRadioButton, MatRadioModule } from "@angular/material/radio";
import { MatIcon } from "@angular/material/icon";
import { MatProgressSpinnerModule } from "@angular/material/progress-spinner";
import { MatInput } from "@angular/material/input";
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { CategoriesResponse } from '../../../../core/interfaces/category.interface';
import { VerifyDialog } from '../../../dialog/verify-dialog/verify-dialog';
import { AutocompleteValidators } from '../../../../core/shared/validators/autocomplete-validator';

@Component({
  selector: 'app-subcategory-form',
  imports: [MatFormField, MatInput, MatIcon, MatSuffix,
    ReactiveFormsModule, MatAnchor, MatIconButton, MatButtonModule,
    MatError, MatProgressSpinnerModule, MatRadioModule,
    MatRadioButton, MatAutocompleteModule],
  templateUrl: './subcategory-form.html',
  styleUrl: './subcategory-form.css',
})
export class SubcategoryForm {

  public isFormOpen = model<boolean | null>(null);
  private fb = inject(NonNullableFormBuilder);
  protected isUrl = signal(false);
  protected imagePreview = signal<string | ArrayBuffer | null>(null);
  private notify = inject(NotificationService);
  private dialog = inject(MatDialog);
  public editableSubcategory = input<Subcategory | null>(null);
  public categories = input<CategoriesResponse[] | null>(null);

  protected subcategoryStore = inject(SubcategoryStore);


  selectedFile!: File;
  subcategoryForm = this.fb.group({
    name: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(100)]],
    description: ['', [Validators.maxLength(300)]],
    imageUrl: ['', [Validators.maxLength(800)]],
    categoryId: [0, [
      Validators.required,
      AutocompleteValidators.requiredInList(
        () => this.categories() || [],
        'id'
      )
    ]],
    image: [null],
    imageProvider: ['']
  });

  protected searchText = signal('');
  protected filteredCategories = computed(() => {
    const allCategories = this.categories() || [];
    const filter = this.searchText().toLowerCase();

    // Si no hay texto, mostramos todas las que ya están cargadas
    if (!filter) return allCategories;

    // Filtramos sobre la lista que ya tenemos en memoria
    return allCategories.filter(cat =>
      cat.name.toLowerCase().includes(filter)
    );
  });

  constructor() {
    effect(() => {
      const editableSubcategory = this.editableSubcategory();
      if (editableSubcategory) {
        this.imagePreview.set(editableSubcategory.imageUrl ?? '');


        this.subcategoryForm.patchValue({
          imageUrl: editableSubcategory.imageUrl,
          name: editableSubcategory.name,
          description: editableSubcategory.description,
          imageProvider: editableSubcategory.imageProvider?.toLocaleLowerCase(),
          categoryId: Number(editableSubcategory.categoryId)
        });
      }
    })
  }



  createSubcategory() {

    if (this.subcategoryForm.invalid) return;
    const formValues = this.subcategoryForm.getRawValue();

    const editableSubcategory = this.editableSubcategory();
    if (editableSubcategory) {

      this.subcategoryStore.updateCategory(editableSubcategory?.id, formValues, this.selectedFile)
        .subscribe({
          next: (res) => {
            // ÉXITO: Mensaje y cerrar
            this.notify.show("¡Subcategoría actualizada!");
            this.isFormOpen.set(false);
          },
          error: (err) => {
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
      this.subcategoryStore.createSubcategory(formValues, this.selectedFile)
        .subscribe({
          next: (res) => {
            // ÉXITO: Mensaje y cerrar
            this.notify.show("¡Subcategoría creada!");
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


  onKey(event: any) {
    this.searchText.set(event.target.value);
  }

  protected resetUrlInput(input: HTMLInputElement) {
    input.value = '';
    this.imagePreview.set(null); // 4. Limpiar con .set()
    this.isUrl.set(false);
    this.subcategoryForm.patchValue({
      imageUrl: ''
    });
  }


  onFileSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    this.isUrl.set(false);

    if (input.files?.length) {
      this.selectedFile = input.files[0];
      const reader = new FileReader();

      reader.onload = () => {
        this.imagePreview.set(reader.result);
      };

      reader.readAsDataURL(this.selectedFile);
    }
  }


  removeImage(fileInput?: HTMLInputElement) {
    this.imagePreview.set(null); // 3. Limpiar con .set()
    this.selectedFile = null as any;
    this.subcategoryForm.patchValue({ image: null });
    this.subcategoryForm.get('imageProvider')?.reset();

    if (fileInput) {
      fileInput.value = '';
    }
  }


  onCancel() {
    this.isFormOpen.set(false);
  }

  protected displayFn(categoryId: number): string {
    if (!categoryId) return '';
    const category = this.categories()?.find(c => c.id === categoryId);
    return category ? category.name : '';
  }

  protected onCategorySelected(event: any) {
    const selected = event.option.value;
    this.subcategoryForm.patchValue({ categoryId: selected });
  }
}
