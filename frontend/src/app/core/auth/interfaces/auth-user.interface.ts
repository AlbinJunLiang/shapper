export interface AuthUser {
    uid: string;
    email: string | null;
    displayName?: string;
    accessToken?: string;
}