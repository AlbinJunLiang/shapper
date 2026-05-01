
export interface PaymentRequestInfo {
    provider: 'STRIPE' | 'PAYPAL' | 'OTHER' | string;
    companyName: string,
    successUrl: string,
    cancelUrl: string,
}