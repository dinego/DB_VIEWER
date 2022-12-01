import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ModalEditValuesComponent } from './modal-edit-values.component';

describe('ModalEditValuesComponent', () => {
  let component: ModalEditValuesComponent;
  let fixture: ComponentFixture<ModalEditValuesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ModalEditValuesComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ModalEditValuesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
