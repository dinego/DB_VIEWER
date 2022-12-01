import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ButtonSecundaryComponent } from './button-secundary.component';

describe('ButtonSecundaryComponent', () => {
  let component: ButtonSecundaryComponent;
  let fixture: ComponentFixture<ButtonSecundaryComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ButtonSecundaryComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ButtonSecundaryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
