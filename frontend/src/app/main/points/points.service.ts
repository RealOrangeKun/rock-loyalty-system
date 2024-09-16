import { Injectable } from '@angular/core';
import { AuthService } from '../../auth/auth.service';
import { BehaviorSubject, tap } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { enviroment } from '../../../env';

@Injectable({
  providedIn: 'root',
})
export class PointsService {
  points: BehaviorSubject<number> = new BehaviorSubject<number>(22);
  exhangeRate: BehaviorSubject<number> = new BehaviorSubject<number>(0);

  constructor(private authService: AuthService, private http: HttpClient) {}

  getPoints() {
    return this.http.get(`${enviroment.apiUrl}/api/credit-points`).pipe(
      tap((reponseData: any) => {
        this.points.next(Number(reponseData.data.points));
      })
    );
  }

  getExchangeRate() {
    return this.http
      .get(
        `${enviroment.apiUrl}/api/admin/restaurants/${enviroment.restaurantId}`
      )
      .pipe(
        tap((responseData: any) => {
          this.exhangeRate.next(
            Number(responseData.data.restaurant.creditPointsSellingRate)
          );
        })
      );
  }
}
