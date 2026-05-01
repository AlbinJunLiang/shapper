import { Component, computed, effect, inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatToolbarModule } from '@angular/material/toolbar';
import { Toolbar } from "./features/toolbar/toolbar";
import { SlidePanel } from "./features/slide-panel/slide-panel";
import { FloatingInfoPanel } from "./features/floating-info-panel/floating-info-panel";
import { Footer } from "./features/footer/footer";
import { MatDialog } from '@angular/material/dialog';
import { CommonModule } from '@angular/common';
import { FilterStore } from './core/services/filter-storage';
import { UserStore } from './core/services/user-store';
import { DialogService } from './core/services/dialog.service';
import { StoreService } from './core/services/store-service';
import { environment } from '../environments/environment.development';
import { Title } from '@angular/platform-browser';
import { DOCUMENT } from '@angular/common';
import { updateFavicon } from './core/shared/utils/favicon.util';
import { LinkType } from './core/enums/link-type.enum';
import { LinkName } from './core/enums/link-name.enum';
import { Status } from './core/enums/status.enum';


@Component({
  selector: 'app-root',
  imports: [RouterOutlet, MatToolbarModule, MatButtonModule,
    MatIconModule, Toolbar, SlidePanel, FloatingInfoPanel,
    Footer, CommonModule],
  templateUrl: './app.html',
  styleUrl: './app.css'
})

export class App {

  readonly dialog = inject(MatDialog);
  public storeService = inject(StoreService);
  protected filterStore = inject(FilterStore);
  private userStore = inject(UserStore);
  private confirmDialogService = inject(DialogService);
  private titleService = inject(Title);
  private document = inject(DOCUMENT);

  protected icoUrl = computed(() => {
    const result = this.storeService.getLink(LinkType.RESOURCE, LinkName.ICON);
    return (result?.url && result.status === Status.ACTIVE)
      ? result.url
      : "shapper.ico";
  });

  constructor() {
    this.filterStore.loadCategoriesFilter();
    this.userStore.initSync();
    this.storeService.loadStore(environment.storeReference);

    effect(() => {
      const store = this.storeService.storeData();
      if (store) {
        this.titleService.setTitle(`${store.name}`);
        updateFavicon(this.document, this.icoUrl());
      } else {
        this.titleService.setTitle('Shapper');
      }
    })

    effect(() => {
      const userData = this.userStore.userSyncData();
      if (userData && userData.data.status !== 'VERIFIED' && this.dialog.openDialogs.length === 0) {
        this.confirmDialogService.openConfirmVerification();
        this.titleService.setTitle(this.storeService.storeData()?.email ?? "");

      }
    });
  } // End of constructor
} //END
