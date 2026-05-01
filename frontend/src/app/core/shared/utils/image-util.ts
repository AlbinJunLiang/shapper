export const convertAnyImageToWebP = (
  file: File,
  quality: number = 1
): Promise<{ blob: Blob; url: string }> => {
  return new Promise((resolve, reject) => {
    if (!file.type.startsWith('image/')) {
      return reject('The file is not a valid image');
    }

    const img = new Image();
    const objectUrl = URL.createObjectURL(file);

    img.src = objectUrl;

    img.onload = () => {
      const canvas = document.createElement('canvas');
      canvas.width = img.naturalWidth;
      canvas.height = img.naturalHeight;

      const ctx = canvas.getContext('2d');
      if (!ctx) {
        URL.revokeObjectURL(objectUrl);
        return reject('Could not get canvas context');
      }

      ctx.drawImage(img, 0, 0);

      canvas.toBlob(
        (blob) => {
          URL.revokeObjectURL(objectUrl);

          if (!blob) return reject('Conversion failed');

          const url = URL.createObjectURL(blob);

          resolve({ blob, url });
        },
        'image/webp',
        quality
      );
    };

    img.onerror = () => {
      URL.revokeObjectURL(objectUrl);
      reject('Unsupported image format');
    };
  });
};