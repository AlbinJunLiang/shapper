export interface UserData {
    id: number;         
    name: string;
    lastName: string;
    roleId: number;
    email: string;
    address: string;
    phoneNumber: string;
    status: 'VERIFIED' | 'REGISTERED' | 'INACTIVE' | 'BANNED'| 'ACTIVE';
}