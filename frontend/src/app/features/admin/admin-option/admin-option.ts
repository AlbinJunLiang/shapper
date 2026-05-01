import { Component, inject } from '@angular/core';
import { MatIcon } from "@angular/material/icon";
import { ToggleSlider } from '../../../core/services/toggle-slider';

@Component({
  selector: 'app-admin-option',
  imports: [MatIcon],
  templateUrl: './admin-option.html',
  styleUrl: './admin-option.css',
})
export class AdminOption {
  protected showSlidePanel = inject(ToggleSlider);
}
