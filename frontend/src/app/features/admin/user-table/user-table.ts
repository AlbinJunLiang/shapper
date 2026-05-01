import { Component, computed, inject, signal } from '@angular/core';
import { MatPaginator, PageEvent } from "@angular/material/paginator";
import { MatProgressSpinner } from '@angular/material/progress-spinner';
import { MatCellDef, MatHeaderCellDef, MatRowDef, MatHeaderRowDef, MatTableModule } from "@angular/material/table";
import { UserService } from '../../../core/services/user-service';
import { UserData } from '../../../core/interfaces/user-data.interface';
import { PagedUsers } from '../../../core/interfaces/paged-user.interfaces';
import { getRolesTranslationKey } from '../../../core/shared/utils/roles';
import { TranslateModule } from '@ngx-translate/core';
import { MatIcon } from "@angular/material/icon";
import { MatMenuModule } from "@angular/material/menu";
import { Status } from '../../../core/enums/status.enum';
import { NotificationService } from '../../../core/services/notification.service';

@Component({
  selector: 'app-user-table',
  imports: [MatCellDef, MatHeaderCellDef, MatPaginator, MatRowDef, 
    MatHeaderRowDef, MatProgressSpinner, MatTableModule, TranslateModule, MatIcon, MatMenuModule],
  templateUrl: './user-table.html',
  styleUrl: './user-table.css',
})
export class UserTable {
  protected displayedColumns: string[] = ['action', 'id', 'name', 'lastName', 'roleId', 'email', 'address', 'phoneNumber', 'status'];
  protected pageSize = signal(10);
  protected currentPage = signal(1);
  protected totalItems = signal(0);
  protected isLoading = signal(false);
  private allUsers = signal<UserData[]>([]);
  public pagedUsers = computed(() => this.allUsers());
  private userService = inject(UserService);
  protected roles = getRolesTranslationKey;
  protected readonly Number = Number;
  private notify = inject(NotificationService);
  public Status = Status; // Para que el HTML lo reconozca


  ngOnInit(): void {
    this.loadOrders();
  }

  private loadOrders() {
    this.isLoading.set(true);
    this.userService.getUsers(this.currentPage(), this.pageSize())
      .subscribe({
        next: (response: PagedUsers) => {
          this.allUsers.set(response.data);
          this.totalItems.set(response.totalCount);
          this.isLoading.set(false);
        },
        error: (err) => {
          console.error('Error al obtener locaciones:', err);
          this.isLoading.set(false);
          this.allUsers.set([]); // Limpiamos en caso de error
        }
      });
  }
  onPageChange(event: PageEvent): void {
    this.currentPage.set(event.pageIndex + 1);
    this.pageSize.set(event.pageSize);
    this.loadOrders();
  }


  toggleUserStatus(user: any, targetStatus?: Status) {
    // 1. Decidir el nuevo estado
    let newStatus: Status;

    if (targetStatus) {
      newStatus = targetStatus;
    } else {
      newStatus = user.status === Status.ACTIVE ? Status.INACTIVE : Status.ACTIVE;
    }

    // 2. Llamada al servicio
    this.userService.updateUserStatus(user.email, newStatus).subscribe({
      next: () => {
        this.allUsers.update(users =>
          users.map(u => u.email === user.email ? { ...u, status: newStatus } : u)
        );

        let msg = 'Estado actualizado';
        if (newStatus === Status.ACTIVE) msg = 'Usuario activado';
        if (newStatus === Status.INACTIVE) msg = 'Usuario desactivado';
        if (newStatus === Status.BANNED) msg = 'Usuario bloqueado';

        this.notify.show(msg);
      },
      error: () => this.notify.show("Error al comunicarse con el servidor")
    });
  }


  protected canDeactivate(status: string | undefined | null): boolean {
    if (!status) return false;
    const allowedStatuses = [Status.ACTIVE, Status.REGISTERED, Status.VERIFIED];
    return allowedStatuses.includes(status.toUpperCase() as Status);
  }

}
