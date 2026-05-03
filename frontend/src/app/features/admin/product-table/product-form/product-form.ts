import { Component, computed, effect, inject, input, model, signal } from '@angular/core';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators, FormsModule, FormControl } from '@angular/forms';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatAnchor, MatButtonModule } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';
import { MatInput, MatInputModule, MatPrefix } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatRadioModule } from '@angular/material/radio';
import { MatError, MatFormField, MatSuffix, MatSelect } from '@angular/material/select';
import { AutocompleteValidators } from '../../../../core/shared/validators/autocomplete-validator';
import { SubcategoryStore } from '../../../../core/services/subcategory-store';

import { ProductStatus } from '../../../../core/enums/product-status.enum';
import { Product } from '../../../../core/interfaces/product.interface';
import { NotificationService } from '../../../../core/services/notification.service';
import { MatDialog } from '@angular/material/dialog';
import { VerifyDialog } from '../../../dialog/verify-dialog/verify-dialog';
import { ProductStore } from '../../../../core/services/product-store';
import { ProductRequest } from '../../../../core/interfaces/product-request.interface';

@Component({
  selector: 'app-product-form',
  imports: [MatFormField, MatInput, MatIcon, MatSuffix,
    ReactiveFormsModule, MatAnchor, MatButtonModule,
    MatError, MatProgressSpinnerModule, MatRadioModule,
     MatInputModule, MatAutocompleteModule,
    MatPrefix, MatSelect, FormsModule],
  templateUrl: './product-form.html'
})
export class ProductForm {

  public Status = ProductStatus;
  private fb = inject(NonNullableFormBuilder);
  public isFormOpen = model<boolean | null>(null);
  protected subcategoryStore = inject(SubcategoryStore);
  protected productStore = inject(ProductStore);
  public subcategories = this.subcategoryStore.subcategories;
  public editableProduct = input<Product | null>(null);
  private notify = inject(NotificationService);
  private dialog = inject(MatDialog);
  protected detailTitle = new FormControl('', [
    Validators.maxLength(100),
    Validators.pattern(/^[a-zA-ZáéíóúÁÉÍÓÚñÑ]+$/) // Solo letras, sin espacios
  ]);
  protected detailValue = new FormControl('', [Validators.maxLength(100)]);

  protected detailsList = signal<Array<{ title: string, value: string }>>([]);

  public selectedFile!: File;
  protected searchText = signal('');


  protected productForm = this.fb.group({
    name: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(100)]],
    description: ['', [Validators.maxLength(300)]],
    price: [0, [Validators.required, Validators.min(0), Validators.max(100000000000000),
    ]],

    taxAmount: [
      0,
      [
        Validators.required,
        Validators.min(0),
        Validators.pattern(/^(100|[0-9]{1,2})(\.\d{1,2})?$/)]
    ],

    discount: [
      0,
      [
        Validators.required,
        Validators.min(0),
        Validators.pattern(/^(100|[0-9]{1,2})(\.\d{1,2})?$/)
      ]
    ],
    status: [ProductStatus.ACTIVE, Validators.required],
    quantity: [0, [Validators.required, Validators.min(0), Validators.max(100000000000000)]],

    subcategoryId: [null as any as number, [
      Validators.required, AutocompleteValidators.requiredInList(
        () => this.subcategories() || [],
        'id'
      )
    ]],
  });


  constructor() {
    this.subcategoryStore.getSubcategories();
    effect(() => {
      const editableProduct = this.editableProduct();
      if (editableProduct) {
        this.productForm.patchValue({
          name: editableProduct.name,
          description: editableProduct.description,
          price: editableProduct.price,
          taxAmount: editableProduct.taxAmount,
          quantity: editableProduct.quantity,
          discount: editableProduct.discount,
          subcategoryId: editableProduct.subcategoryId,
          status: editableProduct.status as ProductStatus
        });

        const mappedDetails = this.formatJsonToDetails(editableProduct.details);
        this.detailsList.set(mappedDetails);
      }
    })
  }


  addDetail() {
    const title = this.detailTitle.value;
    const value = this.detailValue.value;

    if (title && value) {
      this.detailsList.update(list => [...list, { title, value }]);
      this.detailTitle.reset();
      this.detailValue.reset();
    }
  }


  removeDetail(index: number) {
    this.detailsList.update(list =>
      list.filter((_, i) => i !== index)
    );
  }

  protected filteredsubcategories = computed(() => {
    const allSubcategories = this.subcategories() || [];
    const filter = this.searchText().toLowerCase();

    if (!filter) return allSubcategories;
    return allSubcategories.filter(cat =>
      cat.name.toLowerCase().includes(filter)
    );
  });


  onCancel() {
    this.isFormOpen.set(false);
  }

  onKey(event: any) {
    this.searchText.set(event.target.value);
  }

  private formatDetailsToJson(): string {
    const list = this.detailsList();

    const detailsObject = list.reduce((acc, item) => {
      if (item.title?.trim() && item.value?.trim()) {
        const key = item.title.trim();
        acc[key] = item.value.trim();
      }
      return acc;
    }, {} as Record<string, string>);

    return JSON.stringify(detailsObject);
  }

  createProduct() {

    if (this.productForm.invalid) {
      this.productForm.markAllAsTouched();
      return; // Detenemos la ejecución
    }

    const formValues = this.productForm.getRawValue();

    const data: ProductRequest = {
      name: formValues.name,
      description: formValues.description,
      discount: formValues.discount,
      taxAmount: formValues.taxAmount,
      price: formValues.price,
      quantity: formValues.quantity, // No olvides la cantidad
      subcategoryId: formValues.subcategoryId,
      status: formValues.status,
      details: this.formatDetailsToJson() ?? '{}'
    };

    const editableSubcategory = this.editableProduct();
    if (editableSubcategory) {
      this.productStore.updateProduct(data, editableSubcategory.id)
        .subscribe({
          next: (res) => {
            this.notify.show("Producto actualizado!");
            this.isFormOpen.set(false);

          },
          error: (err) => {
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
      this.productStore.createProduct(data)
        .subscribe({
          next: (res) => {
            this.notify.show("Producto creado!");
            this.isFormOpen.set(false);
          },
          error: (err) => {
            console.log(err)
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

  protected displayFn(subcategoryId: number): string {
    if (!subcategoryId) return '';
    const subcategory = this.subcategories()?.find(c => c.id === subcategoryId);
    return subcategory ? subcategory.name : '';
  }

  protected onSubcategorySelected(event: any) {
    const selected = event.option.value;
    this.productForm.patchValue({ subcategoryId: selected });
  }

  private formatJsonToDetails(json: string) {
    if (!json) return [];

    try {
      const obj = JSON.parse(json);

      return Object.entries(obj).map(([key, value]) => ({
        title: key,
        value: String(value)
      }));

    } catch (error) {
      console.error('JSON inválido', error);
      return [];
    }
  }
}
