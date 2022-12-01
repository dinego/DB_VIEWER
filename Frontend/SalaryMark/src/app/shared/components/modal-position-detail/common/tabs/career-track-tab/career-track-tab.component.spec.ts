import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CareerTrackTabComponent } from './career-track-tab.component';

describe('CareerTrackTabComponent', () => {
  let component: CareerTrackTabComponent;
  let fixture: ComponentFixture<CareerTrackTabComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CareerTrackTabComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CareerTrackTabComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
