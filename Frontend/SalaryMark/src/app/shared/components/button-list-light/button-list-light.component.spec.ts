import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { ButtonListLightComponent } from './button-list-light.component';

describe('ButtonListLightComponent', () => {
  let component: ButtonListLightComponent;
  let fixture: ComponentFixture<ButtonListLightComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ ButtonListLightComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ButtonListLightComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
