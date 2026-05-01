import { Component, inject, signal } from '@angular/core';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatTableModule } from '@angular/material/table';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatIcon } from "@angular/material/icon";
import { MatMenuModule } from "@angular/material/menu";
import { IFaq } from '../../../core/interfaces/faq.interface';
import { FaqForm } from "./faq-form/faq-form";
import { FaqStore } from '../../../core/services/faq.sore';
import { VerifyDialog } from '../../dialog/verify-dialog/verify-dialog';
import { NotificationService } from '../../../core/services/notification.service';
import { MatDialog } from '@angular/material/dialog';

@Component({
  selector: 'app-faqs-table',
  imports: [MatPaginatorModule, MatTableModule, MatProgressSpinnerModule, MatIcon, MatMenuModule, FaqForm],
  templateUrl: './faqs-table.html',
  styleUrl: './faqs-table.css',
})
export class FaqsTable {


  protected displayedColumns: string[] = ['actions', 'id', 'question', 'answer', 'status',];
  protected pageSize = signal(10);
  protected currentPage = signal(1);
  protected totalItems = signal(0);
  protected isLoading = signal(false);
  protected faqStore = inject(FaqStore);
  protected isFormOpen = signal(false);
  public editableFaq: IFaq | null = null;
  private notify = inject(NotificationService);
  private dialog = inject(MatDialog);

  ngOnInit(): void {
    this.loadFaqs();
  }

  private loadFaqs() {
    this.faqStore.loadFaqs(this.currentPage(), this.pageSize());
  }

  goToCreate() {
    this.isFormOpen.update(v => !v);
    this.editableFaq = null;
  }

  goToEdit(faq: IFaq) {
    this.editableFaq = faq;
    this.isFormOpen.set(true);
  }

  onPageChange(event: PageEvent): void {
    this.currentPage.set(event.pageIndex + 1);
    this.pageSize.set(event.pageSize);
    this.loadFaqs();
  }


  deleteFaq(faq: IFaq) {
    const dialogRef = this.dialog.open(VerifyDialog, {
      width: '350px',
      data: {
        title: 'Confirmar eliminación',
        message: `¿Estás seguro de eliminar el registro? Esta acción no se puede deshacer.`
      }
    });

    dialogRef.afterClosed().subscribe(result => {

      if (result) {
        this.faqStore.deleteFaq(faq.id).subscribe({
          next: () => {
            this.notify.show("Eliminado con éxito");
          },
          error: (err) => {
            this.dialog.open(VerifyDialog, {
              data: {
                title: 'Error al eliminar',
                message: err.error?.message || 'No se puede borrar.'
              }
            });
          }
        });
      }
    });
  }
}
