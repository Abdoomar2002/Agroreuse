export interface User {
  id: string;
  email: string;
  fullName: string;
  address?: string;
  phoneNumber?: string;
  type: number; // 0 = Farmer, 1 = Factory, 2 = Admin
  createdAt: string;
  isLocked: boolean;
  imagePath?: string;
}

export interface UsersResponse {
  users: User[];
  total: number;
  farmerCount: number;
  factoryCount: number;
}
