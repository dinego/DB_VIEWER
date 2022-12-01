import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { ModalShareTableComponent } from './modal-share-table.component';

describe('ModalShareTableComponent', () => {
  let component: ModalShareTableComponent;
  let fixture: ComponentFixture<ModalShareTableComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ ModalShareTableComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ModalShareTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
