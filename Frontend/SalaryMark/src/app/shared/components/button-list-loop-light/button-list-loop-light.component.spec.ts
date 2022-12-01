import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ButtonListLoopLightComponent } from './button-list-loop-light.component';

describe('ButtonListLoopLightComponent', () => {
  let component: ButtonListLoopLightComponent;
  let fixture: ComponentFixture<ButtonListLoopLightComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ButtonListLoopLightComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ButtonListLoopLightComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
