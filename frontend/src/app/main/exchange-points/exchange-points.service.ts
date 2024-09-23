import { HttpClient } from '@angular/common/http';
import { enviroment } from '../../../env';
import { BehaviorSubject, tap } from 'rxjs';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class ExchangePointsService {
  constructor(private http: HttpClient) {}
  exhangeRate: BehaviorSubject<number> = new BehaviorSubject<number>(0);
  points: BehaviorSubject<number> = new BehaviorSubject<number>(0);

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
