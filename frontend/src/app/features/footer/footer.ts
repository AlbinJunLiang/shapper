import { Component, inject, signal } from '@angular/core';
import { StoreService } from '../../core/services/store-service';
import { Router } from "@angular/router";
import { VerifyDialog } from '../dialog/verify-dialog/verify-dialog';
import { MatDialog } from '@angular/material/dialog';
import { LinkType } from '../../core/enums/link-type.enum';
import { LinkName } from '../../core/enums/link-name.enum';
import { Status } from '../../core/enums/status.enum';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-footer',
  imports: [TranslateModule],
  templateUrl: './footer.html'
})
export class Footer {
  public currentYear = signal(new Date().getFullYear());
  protected storeService = inject(StoreService);
  protected storeInformation = this.storeService.storeData;
  private dialog = inject(MatDialog);
  private router = inject(Router);
  protected LinkName = LinkName;

  ngOnInit() {

  }
  showMessage(link: LinkName) {
    const result = this.storeService.getLink(LinkType.SOCIAL_MEDIA, link);

    if (!result || result.status !== Status.ACTIVE) {
      this.dialog.open(VerifyDialog, {
        width: '250px',
        enterAnimationDuration: '0ms',
        exitAnimationDuration: '0ms',
        data: {
          title: 'Mensaje - Message',
          message: 'ENLACE NO DISPONIBLE (Link not available).'
        }
      });

      this.router.navigate(['']);
      return;
    }

    window.open(result.url, '_blank');
  }

  goToPage(route: string) {
    this.router.navigate([route]);
    window.scrollTo({
      top: 0,
      behavior: 'smooth'
    });
  }

}
