import { Component, inject, signal } from '@angular/core';
import { ToggleSlider } from '../../core/services/toggle-slider';
import { MatIcon } from "@angular/material/icon";
import { MatDialog } from '@angular/material/dialog';
import { AuthService } from '../../core/auth/services/auth-service';
import { CommonModule } from '@angular/common';
import { UserStore } from '../../core/services/user-store';
import { CustomerOption } from "./customer-option/customer-option";
import { GeneralOption } from "./general-option/general-option";
import { AdminOption } from "../admin/admin-option/admin-option";
import { Role } from '../../core/enums/role.enum';

@Component({
  selector: 'app-slide-panel',
  imports: [MatIcon, CommonModule, CustomerOption, GeneralOption, AdminOption],
  templateUrl: './slide-panel.html',
  styleUrl: './slide-panel.css',
})
export class SlidePanel {
  protected showSlidePanel = inject(ToggleSlider);
  readonly dialog = inject(MatDialog);
  protected authService = inject(AuthService);
  protected userStore = inject(UserStore);
  public isLoading = signal(false);
  protected Role = Role;

} //END
