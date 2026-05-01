export enum AuthErrorMessages {
    EMAIL_ALREADY_IN_USE = 'Este correo ya está registrado.',
    WRONG_PASSWORD = 'La contraseña es incorrecta.',
    USER_NOT_FOUND = 'Usuario no encontrado.',
    INVALID_CREDENTIAL = 'AUTH_MESSAGES.INVALID_CREDENTIAL',
    TOO_MANY_REQUESTS = 'AUTH_MESSAGES.TOO_MANY_REQUESTS',
    DEFAULT = 'Ocurrió un error inesperado.',
    
}

export const AuthErrorMap: Record<string, string> = {
    'auth/email-already-in-use': AuthErrorMessages.EMAIL_ALREADY_IN_USE,
    'auth/wrong-password': AuthErrorMessages.WRONG_PASSWORD,
    'auth/user-not-found': AuthErrorMessages.USER_NOT_FOUND,
    'auth/invalid-credential': AuthErrorMessages.INVALID_CREDENTIAL,
    'auth/too-many-requests': AuthErrorMessages.TOO_MANY_REQUESTS,
};