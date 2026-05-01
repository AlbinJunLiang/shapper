export interface OrderPayment {
    id: number;
    transactionReference: string;
    subtotal: number;
    taxAmount: number;
    totalAmount: number;
    paymentMethod: 'PAYPAL-CARD' | 'PAYPAL' | string; // Lo tipamos como string pero sugerimos valores
    paidAt: string | Date; // Viene como string ISO, pero puedes manejarlo como Date
    status: 'COMPLETED' | 'PENDING' | 'FAILED' | string;
}