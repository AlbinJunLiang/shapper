export interface IFaq {
    id: number;
    storeId: number;
    storeName: string;
    storeCode: string;
    question: string;
    answer: string;
    status: string;
    createdAt: string | Date;
    updatedAt: string | Date | null;
}

export interface CreateFaq {
    storeId: number;
    question: string;
    answer: string;
    status: string;
}

export interface UpdateFaq {
    question: string;
    answer: string;
    status: string;
}