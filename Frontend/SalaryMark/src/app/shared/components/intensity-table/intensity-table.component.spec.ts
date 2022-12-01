import { ComponentFixture, TestBed } from '@angular/core/testing';

import { IntensityTableComponent } from './intensity-table.component';

describe('IntensityTableComponent', () => {
  let component: IntensityTableComponent;
  let fixture: ComponentFixture<IntensityTableComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ IntensityTableComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(IntensityTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
