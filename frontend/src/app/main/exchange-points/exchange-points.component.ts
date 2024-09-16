import { Component, OnDestroy, OnInit } from '@angular/core';
import { PointsService } from '../points/points.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-exchange-points',
  templateUrl: './exchange-points.component.html',
  styleUrl: './exchange-points.component.css',
})
export class ExchangePointsComponent implements OnInit, OnDestroy {
  points: number;
  from: number = 0;
  to: number = 0;
  exchangeRate: number = 2;
  private pointsServiceSub: Subscription;
  constructor(private pointsService: PointsService) {}

  ngOnInit(): void {
    this.pointsServiceSub = this.pointsService.points.subscribe((points) => {
      this.points = points;
    });
    // this.pointsService.getPoints().subscribe();
  }

  ngOnDestroy(): void {
    this.pointsServiceSub.unsubscribe();
  }

  fromTyping() {
    this.to = this.exchangeRate * this.from;
  }

  toTyping() {
    this.from = this.to / this.exchangeRate;
  }
}
