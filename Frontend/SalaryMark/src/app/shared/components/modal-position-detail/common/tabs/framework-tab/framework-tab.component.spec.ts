import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FrameworkTabComponent } from './framework-tab.component';

describe('FrameworkTabComponent', () => {
  let component: FrameworkTabComponent;
  let fixture: ComponentFixture<FrameworkTabComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ FrameworkTabComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(FrameworkTabComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
