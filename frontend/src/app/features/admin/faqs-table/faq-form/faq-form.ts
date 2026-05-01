import { Component, effect, inject, input, model } from '@angular/core';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatFormField, MatError, MatSuffix, MatOption, MatSelectModule } from "@angular/material/select";
import { MatIconModule } from "@angular/material/icon";
import { CreateFaq, IFaq } from '../../../../core/interfaces/faq.interface';
import { NotificationService } from '../../../../core/services/notification.service';
import { MatDialog } from '@angular/material/dialog';
import { VerifyDialog } from '../../../dialog/verify-dialog/verify-dialog';
import { StoreService } from '../../../../core/services/store-service';
import { MatProgressSpinner } from "@angular/material/progress-spinner";
import { MatAnchor } from "@angular/material/button";
import { MatInput } from "@angular/material/input";
import { FaqStore } from '../../../../core/services/faq.sore';

@Component({
  selector: 'app-faq-form',
  imports: [MatFormField, MatIconModule, MatError, MatProgressSpinner, 
    ReactiveFormsModule, MatAnchor, MatInput, MatSuffix, MatOption, MatSelectModule],
  templateUrl: './faq-form.html',
  styleUrl: './faq-form.css',
})
export class FaqForm {

  private fb = inject(NonNullableFormBuilder);
  public editableFaq = input<IFaq | null>(null);
  protected faqStore = inject(FaqStore);
  public isFormOpen = model<boolean | null>(null);
  private notify = inject(NotificationService);
  private dialog = inject(MatDialog);
  private store = inject(StoreService);

  faqForm = this.fb.group({
    question: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(500)]],
    answer: ['', [Validators.required, Validators.maxLength(500)]],
    status: ['ACTIVE', [Validators.required]],

  });


  constructor() {
    effect(() => {
      const editable = this.editableFaq();
      if (editable) {
        this.faqForm.patchValue({
          question: editable.question,
          answer: editable.answer,
          status: editable.status?.toUpperCase()
        });
      }
    })
  }

  createFaq() {
    if (this.faqForm.invalid) return;

    const formValues = this.faqForm.getRawValue();

    const editableFaq = this.editableFaq();
    if (editableFaq) {

      this.faqStore.updateFaq(editableFaq.id, formValues)
        .subscribe({
          next: (res) => {
            this.notify.show("Faq actualizado!");
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

      this.faqStore.createFaq(this.makeNewFaq(formValues))
        .subscribe({
          next: (res) => {
            this.notify.show("Faq creada!");
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
    }
  }


  private makeNewFaq(form: any): CreateFaq {
    return {
      storeId: this.store.storeData()?.id ?? 0,
      question: form.question,
      answer: form.answer,
      status: form.status
    }
  }

  onCancel() {
    this.isFormOpen.set(false);
  }
}
