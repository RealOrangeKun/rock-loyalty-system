import { Injectable } from '@angular/core';
import { AuthService } from '../../auth/auth.service';
import { BehaviorSubject, tap } from 'rxjs';
import { HttpClient, HttpParams } from '@angular/common/http';
import { enviroment } from '../../../env';
import { Points, PointsResponse } from './points.interface';

@Injectable({
  providedIn: 'root',
})
export class PointsService {
  constructor(private authService: AuthService, private http: HttpClient) {}

  pointsData: Points[] = [
    {
      receiptId: 1001,
      restaurantId: 200,
      customerId: 501,
      transactionType: 1,
      transactionDate: new Date('2024-01-15'),
      transactionValue: 45.5,
      isExpired: false,
      points: 100,
    },
    {
      receiptId: 1002,
      restaurantId: 201,
      customerId: 502,
      transactionType: 2,
      transactionDate: new Date('2024-02-20'),
      transactionValue: 60.75,
      isExpired: true,
      points: 150,
    },
    {
      receiptId: 1003,
      restaurantId: 200,
      customerId: 503,
      transactionType: 1,
      transactionDate: new Date('2024-03-10'),
      transactionValue: 30.0,
      isExpired: false,
      points: 80,
    },
    {
      receiptId: 1004,
      restaurantId: 202,
      customerId: 504,
      transactionType: 3,
      transactionDate: new Date('2024-04-05'),
      transactionValue: 100.25,
      isExpired: false,
      points: 200,
    },
    {
      receiptId: 1005,
      restaurantId: 203,
      customerId: 505,
      transactionType: 2,
      transactionDate: new Date('2024-05-18'),
      transactionValue: 55.0,
      isExpired: true,
      points: 120,
    },
  ];

  pointsList: BehaviorSubject<Points[]> = new BehaviorSubject<Points[]>(
    this.pointsData
  );

  getPointsList(pageNumber: number, pageSize: number) {
    return this.http
      .get<PointsResponse>(
        `${enviroment.apiUrl}/api/users/${this.authService.currentUser.id}/restaurants/${this.authService.restaurantId}/credit-points-transactions`,
        {
          params: new HttpParams()
            .set('pageNumber', pageNumber)
            .set('pageSize', pageSize),
        }
      )
      .pipe(
        tap((responseData) => {
          const transResponse: Points[] =
            responseData.data.transactionsResponse;
          this.pointsList.next(transResponse);
        })
      );
  }
}
