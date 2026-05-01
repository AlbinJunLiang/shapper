export interface OrderDetail {
  productId: number;
  productName: string;
  description: string;
  productImageUrl: string | null;
  requestedQuantity: number;
  actualQuantity: number;
  basePrice: number;
  discount: number;
  tax: number;
  finalPrice: number;
  subtotal: number;
  status: string;
}