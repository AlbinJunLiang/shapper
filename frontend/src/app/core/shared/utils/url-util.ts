// src/app/shared/utils/url-helper.ts

/**
 * Extrae la ruta a partir de un segmento específico
 * @param fullUrl La URL completa (ej: /products/filter?id=1)
 * @param segment El segmento desde donde queremos cortar (ej: 'filter')
 * @returns La cadena desde el segmento en adelante o vacío si no existe
 */
export const getPathFromSegment = (fullUrl: string, segment: string): string => {
  const segmentPath = `/${segment}`;
  const index = fullUrl.indexOf(segmentPath);

  if (index !== -1) {
    return fullUrl.substring(index + 1);
  }
  
  return '';
};