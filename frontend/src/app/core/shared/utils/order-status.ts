import { OrderStatus } from "../../types/order-status.type";

export function getStatusTranslationKey(status: OrderStatus): string {
  const keys: Record<string, string> = {
    'PENDING':   'ORDER_STATUS.PENDING',
    'PAID':      'ORDER_STATUS.PAID',
    'COMPLETED': 'ORDER_STATUS.COMPLETED',
    'CANCELLED': 'ORDER_STATUS.CANCELLED'
  };

  return keys[status] || status;
}