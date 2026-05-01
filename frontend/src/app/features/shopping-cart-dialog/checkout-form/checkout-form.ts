import { Component, ElementRef, inject, OnInit, output, viewChild } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { toSignal } from '@angular/core/rxjs-interop';
import { map, startWith } from 'rxjs/operators';
import { ShippingMethodType } from '../../../core/types/shipping-method.type';
import { AuthService } from '../../../core/auth/services/auth-service';
import { UserStore } from '../../../core/services/user-store';
import { LocationStore } from '../../../core/services/location-store';
import { ExtraData } from '../../../core/interfaces/extra-data.interface';
import { TranslateModule } from '@ngx-translate/core';
import { environment } from '../../../../environments/environment.development';

@Component({
  selector: 'app-checkout-form',
  standalone: true,
  imports: [ReactiveFormsModule, TranslateModule],
  templateUrl: './checkout-form.html'
})
export class CheckoutForm implements OnInit {
  
  protected currency = environment.currency;
  private fb = inject(FormBuilder);
  public form: FormGroup = this.createForm();
  private authService = inject(AuthService);
  private userStore = inject(UserStore);
  public lastSection = viewChild<ElementRef<HTMLElement>>('lastSection');
  protected shippingOption: ShippingMethodType = '';
  protected locationStore = inject(LocationStore);
  public locations = this.locationStore.locations;
  public isLoading = this.locationStore.isLoading;
  public onPlaceSelected = output<number>();


  public isValid = toSignal(
    this.form.statusChanges.pipe(
      startWith(this.form.status),
      map(status => status === 'VALID')
    ),
    { initialValue: false }
  );

  ngOnInit() {
    this.form.get('shippingMethod')?.valueChanges.subscribe((value: ShippingMethodType) => {
      this.shippingOption = value;
      this.updateValidators(value);
    });
    this.setFormData();
    this.locationStore.loadLocations();
  }


  onLocationChange(event: Event) {
    const selectElement = event.target as HTMLSelectElement;
    const id = Number(selectElement.value);

    if (id) {
      this.onPlaceSelected.emit(id);
    }
  }


  setFormData() {
    const user = this.userStore.userSyncData()?.data;
    if (this.authService.isLoggedIn() && user) {
      this.form.patchValue({
        email: user?.email || '',
        phoneNumber: user?.phoneNumber || '',
        name: user?.name || '',
        lastName: user?.lastName || '',
        address: user?.address || '',
      });
    }
  }

  getData(): ExtraData | null {
    if (!this.isValid()) {
      this.scrollToPayment();
      this.form.markAllAsTouched();
      return null;
    }
    return this.form.value;
  }

  scrollToPayment() {
    const element = this.lastSection()?.nativeElement;
    element?.scrollIntoView({
      behavior: 'smooth',
      block: 'start'
    });
  }

  protected activeLocations() {
    return this.locations().filter(l => l.status === 'ACTIVE');
  }


  private updateValidators(option: ShippingMethodType) {
    const address = this.form.get('address');
    const postal = this.form.get('postalCode');

    if (option === 'STORE') {
      address?.clearValidators();
      postal?.clearValidators();
    } else {
      address?.setValidators([Validators.required, Validators.maxLength(300)]);
      postal?.setValidators([Validators.required, Validators.maxLength(15)]);
    }
    address?.updateValueAndValidity();
    postal?.updateValueAndValidity();
  }

  private createForm(): FormGroup {
    return this.fb.group({
      email: ['', [Validators.required, Validators.email, Validators.maxLength(300)]],
      phoneNumber: ['', [Validators.required, Validators.minLength(4), Validators.maxLength(20), Validators.pattern(/^[0-9]+$/)]],
      name: ['', [Validators.required, Validators.maxLength(100)]],
      lastName: ['', [Validators.required, Validators.maxLength(100)]],
      address: ['', [Validators.required, Validators.maxLength(300)]],
      place: ['', [Validators.required]],
      postalCode: ['', [Validators.required, Validators.maxLength(15)]],
      shippingMethod: ['', [Validators.required]]
    });
  }
}