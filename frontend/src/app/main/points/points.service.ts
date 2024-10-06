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
  pointsList: BehaviorSubject<Points[]> = new BehaviorSubject<Points[]>(null);
  constructor(private authService: AuthService, private http: HttpClient) {}

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
          const points: Points[] = [];
          const transResponse = responseData.data.transactionsResponse;

          transResponse.forEach((x) => {
            points.push({
              isExpired: x.isExpired,
              points: x.points,
              transactionValue: x.transactionValue,
            });
          });

          this.pointsList.next(points);
        })
      );
  }
}
