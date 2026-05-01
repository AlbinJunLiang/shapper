export interface SubcategoryRequest {
  name: string;
  description: string;
  categoryId: number;
  imageProvider?: string;
  imageUrl?: string;
  imageId?: string;
}