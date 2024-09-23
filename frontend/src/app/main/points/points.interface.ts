export interface Points {
  transactionValue: 100.0;
  isExpired: false;
  points: 100;
}

export interface metaData {
  totalPages: number;
  totalItems: number;
  pageNumber: number;
  pageSize: number;
}

export interface PointsResponse {
  success: true;
  message: string;
  data: {
    transactions: Points[];
    metadata: metaData;
  };
}
