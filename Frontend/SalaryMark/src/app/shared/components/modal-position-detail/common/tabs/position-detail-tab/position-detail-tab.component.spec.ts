import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PositionDetailTabComponent } from './position-detail-tab.component';

describe('PositionDetailTabComponent', () => {
  let component: PositionDetailTabComponent;
  let fixture: ComponentFixture<PositionDetailTabComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PositionDetailTabComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PositionDetailTabComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
