import { ComponentFixture, TestBed } from '@angular/core/testing';

import { IntensityBarComponent } from './intensity-bar.component';

describe('IntensityBarComponent', () => {
  let component: IntensityBarComponent;
  let fixture: ComponentFixture<IntensityBarComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ IntensityBarComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(IntensityBarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
