import { Component, effect, ElementRef, inject, input, output, viewChild } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { PaymentMethodType } from '../../../core/types/payment-method.type';
import { LocationStore } from '../../../core/services/location-store';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-order-payment-card',
  imports: [ReactiveFormsModule, FormsModule, TranslateModule],
  templateUrl: './order-payment-card.html',
})
export class OrderPaymentCard {
  public checkoutForm = input<any>();
  public paymentSection = viewChild<ElementRef<HTMLElement>>('paymentSection');
  private locationStore = inject(LocationStore);
  private formBuilder = inject(FormBuilder);
  protected methodChanged = output<string>();

  form!: FormGroup;
  paymentMethod: PaymentMethodType = 'none';


  public disabled = input<boolean>(false);

  constructor() {
    effect(() => {
      const isCurrentlyDisabled = this.disabled();
      if (this.form) {
        isCurrentlyDisabled ? this.form.disable() : this.form.enable();
      }
    });
  }
  get selectedPaymentMethod(): string {
    return this.form.get('paymentMethod')?.value ?? '';
  }

  ngOnInit() {
    this.form = this.formBuilder.group({
      paymentMethod: [
        '',
        [Validators.required]
      ]
    });
    this.form.get('paymentMethod')?.valueChanges.subscribe(value => {
      this.methodChanged.emit(value ?? '');
    });
  }

  getLocationName(id: string): string {
    return this.locationStore.locations().find(l => l.id === Number(id))?.name ?? '-';
  }

  isPaymentMethodSelected() {
    const control = this.form.get('paymentMethod');

    if (control?.invalid) {
      control.markAsTouched();

      // El scroll suave
      this.paymentSection()?.nativeElement.scrollIntoView({
        behavior: 'smooth',
        block: 'center'
      });
      return false; // Indica que la validación falló
    }
    return true;
  }

} //END
