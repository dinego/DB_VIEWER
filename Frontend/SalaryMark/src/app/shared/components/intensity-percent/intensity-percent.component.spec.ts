import { ComponentFixture, TestBed } from '@angular/core/testing';

import { IntensityPercentComponent } from './intensity-percent.component';

describe('IntensityPercentComponent', () => {
  let component: IntensityPercentComponent;
  let fixture: ComponentFixture<IntensityPercentComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ IntensityPercentComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(IntensityPercentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
