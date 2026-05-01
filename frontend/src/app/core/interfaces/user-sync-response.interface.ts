import { UserData } from "./user-data.interface";

export interface UserSyncResponse {
  message: string;
  data: UserData;
  isNew: boolean;
}