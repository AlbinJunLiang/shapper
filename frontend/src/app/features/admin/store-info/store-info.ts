import { Component, effect, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { StoreService } from '../../../core/services/store-service';
import { StoreUpdate } from '../../../core/interfaces/store-update.interface';
import { NotificationService } from '../../../core/services/notification.service';
import { MatProgressSpinner } from "@angular/material/progress-spinner";

@Component({
  selector: 'app-store-info',
  standalone: true,
  imports: [MatInputModule, MatFormFieldModule, ReactiveFormsModule, MatProgressSpinner],
  templateUrl: './store-info.html',
  styleUrl: './store-info.css',
})
export class StoreInfo {

  protected storeService = inject(StoreService);
  protected storeInformation = this.storeService.storeData;
  private fb = inject(FormBuilder);
  private notifyService = inject(NotificationService);


  formGroup = this.fb.group({
    name: ['', [Validators.required, Validators.maxLength(300)]],
    email: ['', [Validators.required, Validators.email, Validators.maxLength(300)]],
    description: ['', [Validators.required, Validators.maxLength(300)]],
    phoneNumber: ['', [Validators.required, Validators.maxLength(15), Validators.pattern('^[0-9]*$')]],
    address: ['', [Validators.required, Validators.maxLength(300)]],
  });


  constructor() {
    effect(() => {
      this.setForm();
    });
  }

  setForm() {
    this.formGroup.patchValue({
      name: this.storeInformation()?.name ?? '',
      description: this.storeInformation()?.description ?? '',
      phoneNumber: this.storeInformation()?.phoneNumber ?? '',
      address: this.storeInformation()?.mainLocation ?? '',
      email: this.storeInformation()?.email ?? ''
    });
  }

  

  async updateStore() {
    const { name, description, phoneNumber, address, email } = this.formGroup.value;

    const data: StoreUpdate = {
      name: name ?? '',
      description: description ?? '',
      phoneNumber: phoneNumber ?? '',
      mainLocation: address ?? '',
      email: email ?? ''
    };

    const currentInfo = this.storeInformation();
    if (!currentInfo) return;


    this.storeService.updateStore(Number(currentInfo.id), data as StoreUpdate).subscribe({
      next: () => this.notifyService.show('Actualizado'),
      error: () => this.notifyService.show('Ha ocurrido un error.')
    });
  }
}
