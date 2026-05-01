import { Component, computed, inject } from '@angular/core';
import { MatIcon } from "@angular/material/icon";
import { MatToolbar } from "@angular/material/toolbar";
import { MatButtonModule } from "@angular/material/button";
import { ToggleSlider } from '../../core/services/toggle-slider';
import { MatMenuModule } from '@angular/material/menu';
import { Router, RouterLink } from "@angular/router";
import { TranslateModule } from '@ngx-translate/core';
import { MatDialog } from '@angular/material/dialog';
import { AuthService } from '../../core/auth/services/auth-service';
import { AuthDialog } from '../../core/auth/ui/auth-dialog/auth-dialog';
import { UserStore } from '../../core/services/user-store';
import { LanguageService } from '../../core/services/language.service';
import { Role } from '../../core/enums/role.enum';
import { StoreService } from '../../core/services/store-service';
import { LinkType } from '../../core/enums/link-type.enum';
import { LinkName } from '../../core/enums/link-name.enum';
import { Status } from '../../core/enums/status.enum';

@Component({
  selector: 'app-toolbar',
  imports: [MatIcon, MatToolbar, MatButtonModule, RouterLink, MatMenuModule, TranslateModule],
  templateUrl: './toolbar.html',
  styleUrl: './toolbar.css',
})
export class Toolbar {
  protected showSlidePanel = inject(ToggleSlider);
  private router = inject(Router);
  readonly dialog = inject(MatDialog);
  public authService = inject(AuthService);
  public userStore = inject(UserStore);
  private storeService = inject(StoreService);
  public langService = inject(LanguageService);
  protected Role = Role;
  protected imageUrl = computed(() => {
    const result = this.storeService.getLink(LinkType.RESOURCE, LinkName.IMAGE);
    return (result?.url && result.status === Status.ACTIVE)
      ? result.url
      : "Shapper.png";
  });

  get currentPath(): string {
    return this.router.url.replace('/', '') || 'home';
  }


  protected toggleLang() {
    const newLang = this.langService.currentLang() === 'es' ? 'en' : 'es';
    this.langService.changeLanguage(newLang);
  }

  protected openAuthDialog(mode: 'login' | 'register') {
    this.dialog.closeAll();
    this.dialog.open(AuthDialog, {
      enterAnimationDuration: '100ms',
      exitAnimationDuration: '100ms',
      panelClass: 'custom-tailwind-dialog',
      backdropClass: 'custom-backdrop',
      data: {
        mode: mode
      }
    });
    this.showSlidePanel.close();
  }
}  //END
