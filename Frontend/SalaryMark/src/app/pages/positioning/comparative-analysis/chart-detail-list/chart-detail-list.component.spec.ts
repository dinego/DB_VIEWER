import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ChartDetailListComponent } from './chart-detail-list.component';

describe('ChartDetailListComponent', () => {
  let component: ChartDetailListComponent;
  let fixture: ComponentFixture<ChartDetailListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ChartDetailListComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ChartDetailListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
