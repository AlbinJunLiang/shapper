/**
 * Actualiza el favicon del navegador de forma dinámica.
 * @param document Referencia al objeto DOCUMENT de Angular
 * @param url URL de la nueva imagen (ico, png, svg)
 */
export const updateFavicon = (document: Document, url: string): void => {
  if (!document || !url) return;

  const head = document.head;
  let link = document.querySelector("link[rel*='icon']") as HTMLLinkElement;

  if (!link) {
    link = document.createElement('link');
    link.rel = 'icon';
    head.appendChild(link);
  }

  // Opcional: Soporte para diferentes tipos de imagen
  if (url.endsWith('.png')) link.type = 'image/png';
  if (url.endsWith('.svg')) link.type = 'image/svg+xml';

  link.href = url;
};