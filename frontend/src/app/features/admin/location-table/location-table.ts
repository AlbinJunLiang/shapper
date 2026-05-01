import { Component, computed, inject, signal } from '@angular/core';
import { MatCellDef, MatHeaderCellDef, MatRowDef, MatHeaderRowDef, MatTableModule } from "@angular/material/table";
import { MatPaginator, PageEvent } from "@angular/material/paginator";
import { MatProgressSpinner } from "@angular/material/progress-spinner";
import { CreateLocation, LocationResponse } from '../../../core/interfaces/location.interface';
import { LocationService } from '../../../core/services/location-service';
import { PagedLocations } from '../../../core/interfaces/paged-location.interface';
import { MatIcon } from "@angular/material/icon";
import { MatAnchor } from "@angular/material/button";
import { Status } from '../../../core/enums/status.enum';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatAutocompleteModule } from "@angular/material/autocomplete";
import { MatFormField, MatSelect, MatError, MatSuffix, MatPrefix } from "@angular/material/select";
import { MatInput } from "@angular/material/input";
import { NotificationService } from '../../../core/services/notification.service';
import { MatMenuModule } from "@angular/material/menu";
import { MatDialog } from '@angular/material/dialog';
import { VerifyDialog } from '../../dialog/verify-dialog/verify-dialog';

@Component({
  selector: 'app-location-table',
  imports: [MatCellDef, MatHeaderCellDef, MatPaginator, MatRowDef, MatHeaderRowDef,
     MatProgressSpinner, MatTableModule, MatIcon, MatAnchor, MatAutocompleteModule, 
     MatFormField, MatSelect, ReactiveFormsModule, MatInput, MatError, MatMenuModule, MatSuffix, MatPrefix],
  templateUrl: './location-table.html',
  styleUrl: './location-table.css',
})
export class LocationTable {

  protected displayedColumns: string[] = ['action', 'id', 'name', 'address', 'type', 'cost', 'status'];
  protected pageSize = signal(10);
  protected currentPage = signal(1);
  protected totalItems = signal(0);
  protected isLoading = signal(false);
  private allLocations = signal<LocationResponse[]>([]);
  public pagedLocations = computed(() => this.allLocations());
  private locationService = inject(LocationService);
  public Status = Status;
  protected isFormOpen = false;
  protected editingId = signal<number | null>(null);
  protected isEditing = computed(() => this.editingId() !== null);
  private notify = inject(NotificationService);
  private dialog = inject(MatDialog);

  private fb = inject(NonNullableFormBuilder);
  locationForm = this.fb.group({
    name: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(100)]],
    address: ['', [Validators.required, Validators.maxLength(300)]],
    type: ['Otro', [Validators.required, Validators.maxLength(50)]],
    cost: [0, [Validators.required, Validators.min(0)]],
    status: [Status.ACTIVE, [Validators.required]]
  });

  ngOnInit(): void {
    this.loadOrders();
  }


  private loadOrders() {
    this.isLoading.set(true);
    this.locationService.getLocations(this.currentPage(), this.pageSize())
      .subscribe({
        next: (response: PagedLocations) => {
          this.allLocations.set(response.data);
          this.totalItems.set(response.totalCount);
          this.isLoading.set(false);
        },
        error: (err) => {
          console.error('Error al obtener locaciones:', err);
          this.isLoading.set(false);
          this.allLocations.set([]); // Limpiamos en caso de error
        }
      });
  }


  save() {
    if (this.locationForm.invalid) {
      this.locationForm.markAllAsTouched();
      return;
    }
    const formData = this.locationForm.getRawValue() as CreateLocation;
    this.isLoading.set(true); // Opcional: mostrar spinner mientras guarda

    const currentId = this.editingId();
    if (currentId) {

      this.locationService.updateLocation(formData, currentId).subscribe({
        next: (response) => {
          this.resetForm();
          this.isFormOpen = false;
          this.loadOrders();
          this.allLocations.update(list =>
            list.map(loc => loc.id === currentId ? { ...loc, ...response } : loc)
          );
        },
        error: (err) => {
          this.notify.show("Ha ocurrido un error.");
          this.isLoading.set(false);
        }
      });
    } else {
      this.locationService.createLocation(formData).subscribe({
        next: (response) => {
          this.resetForm();
          this.isFormOpen = false;
          this.loadOrders();
        },
        error: (err) => {
          this.notify.show("Ha ocurrido un error.");
          this.isLoading.set(false);
        }
      });
    }
  }


  deleteLocation(location: LocationResponse) {
    const dialogRef = this.dialog.open(VerifyDialog, {
      width: '350px',
      data: {
        title: 'Confirmar eliminación',
        message: `¿Estás seguro de que deseas eliminar "${location.name}"? Esta acción no se puede deshacer.`
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.isLoading.set(true);

        this.locationService.deleteLocation(location.id).subscribe({
          next: () => {
            this.allLocations.update(list => list.filter(loc => loc.id !== location.id));
            this.totalItems.update(total => total - 1);

            this.isLoading.set(false);
          },
          error: (err) => {
            this.isLoading.set(false);

            this.dialog.open(VerifyDialog, {
              width: '350px',
              data: {
                title: 'Aviso',
                message: `Error al eliminar, verifique que no este asociado con un producto.`
              }
            });

          }
        });
      }
    });
  }

  private resetForm() {
    this.locationForm.reset({
      status: Status.ACTIVE,
      cost: 0,
      type: 'Otro'
    });
  }


  // Ejemplo: Cargar datos para editar
  goToEdit(location: LocationResponse) {
    this.editingId.set(location.id);
    this.locationForm.patchValue({
      name: location.name,
      address: location.address,
      type: location.type,
      cost: location.cost,
      status: location.status as Status // Aseguramos que el string sea tratado como el Enum Status
    });
    this.isFormOpen = true;
  }

  onPageChange(event: PageEvent): void {
    this.currentPage.set(event.pageIndex + 1);
    this.pageSize.set(event.pageSize);
    this.loadOrders();
  }
}
