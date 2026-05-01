import { Component, effect, inject, signal, untracked } from '@angular/core';
import { StoreService } from '../../../core/services/store-service';
import { StoreLinkService } from '../../../core/services/store-link.service';
import { StoreLinkUpdate } from '../../../core/interfaces/store-link-update.interface';
import { NotificationService } from '../../../core/services/notification.service';
import { LinkType } from '../../../core/enums/link-type.enum';
import { LinkName } from '../../../core/enums/link-name.enum';
import { Status } from '../../../core/enums/status.enum';

export interface ILinkForm {
  id: number;
  title: string;
  url: string;
  placeHolder: string;
  icon: string;
  name: string;
  type: string;
  isActive: boolean;
}


@Component({
  selector: 'app-link-form',
  imports: [],
  templateUrl: './link-form.html',
  styleUrl: './link-form.css',
})
export class LinkForm {

  protected storeInformationService = inject(StoreService);
  protected links = signal<ILinkForm[] | []>([]);
  protected isLoading = false;
  protected storeLinkService = inject(StoreLinkService);
  private notify = inject(NotificationService);


  constructor() {

    effect(() => {
      let success = this.storeInformationService.getLink(LinkType.PAYMENT, LinkName.SUCCESS) || null;
      let cancel = this.storeInformationService.getLink(LinkType.PAYMENT, LinkName.CANCEL) || null;
      let icon = this.storeInformationService.getLink(LinkType.RESOURCE, LinkName.ICON) || null;
      let image = this.storeInformationService.getLink(LinkType.RESOURCE, LinkName.IMAGE) || null;
      let whatsApp = this.storeInformationService.getLink(LinkType.SOCIAL_MEDIA, LinkName.WHATSAPP) || null;
      let facebook = this.storeInformationService.getLink(LinkType.SOCIAL_MEDIA, LinkName.FACEBOOK) || null;
      let instagram = this.storeInformationService.getLink(LinkType.SOCIAL_MEDIA, LinkName.INSTAGRAM) || null;
      let twitter = this.storeInformationService.getLink(LinkType.SOCIAL_MEDIA, LinkName.TWITTER) || null;
      let guide = this.storeInformationService.getLink(LinkType.OTHER, LinkName.PAGE_GUIDE) || null;
      let storeLink = this.storeInformationService.getLink(LinkType.OTHER, LinkName.STORE_LINK) || null;



      untracked(() => {
        this.links.set([{
          id: success?.id ?? -1,
          title: "Enlace de pago exitoso",
          url: success?.url ?? '',
          placeHolder: "https://success.com",
          icon: '',
          name: success?.name ?? LinkName.SUCCESS,
          type: success?.type ?? LinkType.PAYMENT,
          isActive: success ? this.isActive(success.status) : false

        },
        {
          id: cancel?.id ?? -2,
          title: "Enlace de pago cancelado",
          url: cancel?.url ?? '',
          placeHolder: "https://cancel.com",
          icon: '',
          name: cancel?.name ?? LinkName.CANCEL,
          type: cancel?.type ?? LinkType.PAYMENT,
          isActive: this.isActive(cancel?.status) ?? false
        },
        {
          id: icon?.id ?? -3,
          title: "Enlace de icono de la tienda",
          url: icon?.url ?? '',
          placeHolder: "https://ico.icon",
          icon: '',
          name: icon?.name ?? LinkName.ICON,
          type: icon?.type ?? LinkType.RESOURCE,
          isActive: this.isActive(icon?.status) ?? false
        },
        {
          id: image?.id ?? -4,
          title: "Enlace de la imagen de la tienda",
          url: image?.url ?? '',
          placeHolder: "https://image.png",
          icon: '',
          name: image?.name ?? LinkName.IMAGE,
          type: image?.type ?? LinkType.RESOURCE,
          isActive: this.isActive(image?.status) ?? false
        },
        {
          id: whatsApp?.id ?? -5,
          title: "Enlace de WhatsApp de la tienda",
          url: whatsApp?.url ?? '',
          placeHolder: "https://whatsapp.com",
          icon: '',
          name: whatsApp?.name ?? LinkName.IMAGE,
          type: whatsApp?.type ?? LinkType.SOCIAL_MEDIA,
          isActive: this.isActive(whatsApp?.status) ?? false
        },
        {
          id: facebook?.id ?? -6,
          title: "Enlace de Facebook",
          url: facebook?.url ?? '',
          placeHolder: "https://facebook.com",
          icon: '',
          name: facebook?.name ?? LinkName.FACEBOOK,
          type: facebook?.type ?? LinkType.SOCIAL_MEDIA,
          isActive: this.isActive(facebook?.status) ?? false
        },
        {
          id: instagram?.id ?? -7,
          title: "Enlace de Instagram",
          url: instagram?.url ?? '',
          placeHolder: "https://instragram.com",
          icon: '',
          name: instagram?.name ?? LinkName.INSTAGRAM,
          type: instagram?.type ?? LinkType.SOCIAL_MEDIA,
          isActive: this.isActive(instagram?.status) ?? false
        },
        {
          id: twitter?.id ?? -8,
          title: "Enlace de Twitter",
          url: twitter?.url ?? '',
          placeHolder: "https://twitter.com",
          icon: '',
          name: twitter?.name ?? LinkName.TWITTER,
          type: twitter?.type ?? LinkType.SOCIAL_MEDIA,
          isActive: this.isActive(twitter?.status) ?? false
        },

        {
          id: guide?.id ?? -9,
          title: "Enlace de guía",
          url: guide?.url ?? '',
          placeHolder: "https://",
          icon: '',
          name: guide?.name ?? LinkName.PAGE_GUIDE,
          type: guide?.type ?? LinkType.OTHER,
          isActive: this.isActive(guide?.status) ?? false
        },
          {
          id: storeLink?.id ?? -10,
          title: "Enlace de tienda",
          url: storeLink?.url ?? '',
          placeHolder: "https://store...",
          icon: '',
          name: storeLink?.name ?? LinkName.STORE_LINK,
          type: storeLink?.type ?? LinkType.OTHER,
          isActive: this.isActive(storeLink?.status) ?? false
        }
        ])
      });
    });
  }



  processUpsert(link: ILinkForm, newUrl: string) {

    let id: number | null = link.id;
    if (id != null && id <= 0) {
      id = null;
    }

    if (!this.isValidUrl(newUrl)) {
      this.notify.show('La URL no es válida');
      return;
    }

    this.isLoading = true;

    const payload: StoreLinkUpdate = {
      storeId: this.storeInformationService.storeData()?.id ?? 0,
      name: link.name,
      url: newUrl,
      type: link.type,
      status: Status.ACTIVE
    };



    this.storeLinkService.upsertStoreLink(id, payload).subscribe({
      next: (res) => {
        this.isLoading = false;
        this.notify.show(id && id > 0 ? '¡Actualizado correctamente!' : '¡Creado correctamente!');
        this.updateLink(link.id ?? -1, res.url, res.id, true);
      },
      error: (err) => {
        this.isLoading = false;
        console.error('Error en la operación:', err);
        const errorMsg = err.error?.message || 'Ocurrió un error inesperado';
        this.notify.show(errorMsg);
      }
    });
  }

  processToggleStatus(id: number) {
    this.storeLinkService.toggleStatus(id).subscribe({
      next: (updatedLink) => {
        const status = updatedLink.status;
        const actualStatus = status === Status.ACTIVE;
        this.links.update(currentLinks =>

          currentLinks.map(link =>
            link.id === id ? { ...link, isActive: actualStatus } : link)
        );

        this.notify.show(`Se ha ${actualStatus ? "activado" : "desactivado"} estado.`);

      },
      error: (err) => {
        this.notify.show("No se pudo cambiar el estado.");
      }
    });
  }


  private isActive(status?: string): boolean {
    return status?.toUpperCase() === Status.ACTIVE;
  }

  updateByUrl(oldUrl: string, newUrl: string) {
    this.links.update(currentLinks =>
      currentLinks.map(link =>
        link.url === oldUrl ? { ...link, url: newUrl } : link
      )
    );
  }


  updateLink(id: number, url: string, newId: number, status: boolean) {
    this.links.update(currentLinks =>
      currentLinks.map(link =>
        link.id === id
          ? { ...link, url, id: newId, isActive: status }
          : link
      )
    );
  }

  addLink(
    id: number,
    title: string,
    url: string,
    placeHolder: string,
    icon: string,
    name: string,
    type: string,
    isActive: boolean) {

    const newLink: ILinkForm = {
      id,
      title,
      url,
      placeHolder,
      icon,
      name,
      type,
      isActive
    };
    this.links.update(currentLinks => [...currentLinks, newLink]);
  }


  toggleLinkStatus(id: number) {
    this.links.update(currentLinks =>
      currentLinks.map(link =>
        link.id === id ? { ...link, isActive: !link.isActive } : link
      )
    );
  }

  isValidUrl(url: string): boolean {
    try {
      if (!url) return true;
      const newUrl = new URL(url);
      return newUrl.protocol === 'http:' || newUrl.protocol === 'https:';
    } catch (err) {
      return false;
    }
  }

  handleCancel(link: ILinkForm, inputElement: HTMLInputElement) {
    if (link.id < 0) {
      inputElement.value = '';
    } else {
      inputElement.value = link.url;
    }

    this.links.update(current =>
      current.map(l => l.id === link.id ? { ...l } : l)
    );
  }

}
