import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { PositioningNavHeaderComponent } from './positioning-nav-header.component';

describe('PositioningNavHeaderComponent', () => {
  let component: PositioningNavHeaderComponent;
  let fixture: ComponentFixture<PositioningNavHeaderComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ PositioningNavHeaderComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PositioningNavHeaderComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
