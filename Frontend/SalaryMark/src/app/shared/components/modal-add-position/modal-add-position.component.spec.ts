import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ModalAddPositionComponent } from './modal-add-position.component';

describe('ModalAddPositionComponent', () => {
  let component: ModalAddPositionComponent;
  let fixture: ComponentFixture<ModalAddPositionComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ModalAddPositionComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ModalAddPositionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
