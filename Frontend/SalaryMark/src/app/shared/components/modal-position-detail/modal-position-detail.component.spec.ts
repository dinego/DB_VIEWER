import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ModalPositionDetailComponent } from './modal-position-detail.component';

describe('ModalPositionDetailComponent', () => {
  let component: ModalPositionDetailComponent;
  let fixture: ComponentFixture<ModalPositionDetailComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ModalPositionDetailComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ModalPositionDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
