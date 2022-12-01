import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ConfirmModalEditPositionComponent } from './confirm-modal-edit-position.component';

describe('ConfirmModalEditPositionComponent', () => {
  let component: ConfirmModalEditPositionComponent;
  let fixture: ComponentFixture<ConfirmModalEditPositionComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ConfirmModalEditPositionComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ConfirmModalEditPositionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
