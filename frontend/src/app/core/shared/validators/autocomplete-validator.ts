import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

export class AutocompleteValidators {
  /**
   * @param getList Una función que retorna la lista (ej: () => this.store.categories())
   * @param key La llave a comparar (id)
   */
  static requiredInList(getList: () => any[], key: string = 'id'): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      const value = control.value;

      // Si no hay valor, no validamos (para eso está Validators.required)
      if (value === null || value === undefined || value === '') return null;

      const list = getList();

      if (!list || list.length === 0) return null;

      const isValid = list.some(item => 
        item[key] == value || 
        item.name?.toString().toLowerCase() === value.toString().toLowerCase()
      );

      return isValid ? null : { invalidAutocomplete: true };
    };
  }
}