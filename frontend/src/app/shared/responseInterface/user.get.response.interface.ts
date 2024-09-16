export interface UserInterface {
  success: boolean;
  message: string;
  data: {
    accessToken: string;
    user: {
      id: string;
      email: string;
      phoneNumber: string;
      restaurantId: string;
      name: string;
    };
  };
}
