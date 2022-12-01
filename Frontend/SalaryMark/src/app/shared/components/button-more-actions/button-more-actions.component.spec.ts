import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { ButtonMoreActionsComponent } from './button-more-actions.component';

describe('ButtonMoreActionsComponent', () => {
  let component: ButtonMoreActionsComponent;
  let fixture: ComponentFixture<ButtonMoreActionsComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ ButtonMoreActionsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ButtonMoreActionsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
