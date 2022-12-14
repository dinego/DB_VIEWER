import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { PositioningComponent } from './positioning.component';

describe('PositioningComponent', () => {
  let component: PositioningComponent;
  let fixture: ComponentFixture<PositioningComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ PositioningComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PositioningComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
