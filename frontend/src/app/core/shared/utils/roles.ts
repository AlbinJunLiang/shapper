import { Role } from "../../enums/role.enum";

export function getRolesTranslationKey(role: number): string {
  const keys: Record<number, string> = {
    [Role.ADMIN]: 'ROLES.ADMIN',
    [Role.CUSTOMER]: 'ROLES.CUSTOMER'
  };

  return keys[role] || '';
}