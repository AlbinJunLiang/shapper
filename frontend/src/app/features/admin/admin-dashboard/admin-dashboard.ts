import { Component, inject } from '@angular/core';
import { RouterLink } from "@angular/router";
import { MatIcon } from "@angular/material/icon";
import { StoreService } from '../../../core/services/store-service';
import { LinkType } from '../../../core/enums/link-type.enum';
import { LinkName } from '../../../core/enums/link-name.enum';
import { NotificationService } from '../../../core/services/notification.service';
import { Status } from '../../../core/enums/status.enum';

@Component({
  selector: 'app-admin-dashboard',
  imports: [RouterLink, MatIcon],
  templateUrl: './admin-dashboard.html',
  styleUrl: './admin-dashboard.css',
})
export class AdminDashboard {

  private storeService = inject(StoreService);
  private notify = inject(NotificationService);

protected openGuide() {
  const result = this.storeService.getLink(LinkType.OTHER, LinkName.PAGE_GUIDE);

  // 1. Verificamos que el link exista, tenga URL y esté activo
  if (!result?.url || result.status !== Status.ACTIVE) {
    this.notify.show('Enlace no disponible');
    return;
  }

  window.open(result.url, '_blank', 'noopener,noreferrer');
}
}
