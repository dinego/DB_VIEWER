import { Component, Input, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-slider-card',
  templateUrl: './slider-card.component.html',
  styleUrls: ['./slider-card.component.scss'],
})
export class SliderCardComponent implements OnInit {
  @Input()
  imageLink: string;

  @Input()
  label: string;

  @Input()
  routeLink: string;

  constructor(private router: Router) {}

  ngOnInit(): void {}

  onClickRoute(route: string) {
    this.router.navigate([route]);
  }
}
