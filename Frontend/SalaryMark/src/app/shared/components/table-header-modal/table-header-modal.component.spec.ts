import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { TableHeaderModalComponent } from './table-header-modal.component';

describe('TableHeaderModalComponent', () => {
  let component: TableHeaderModalComponent;
  let fixture: ComponentFixture<TableHeaderModalComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ TableHeaderModalComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TableHeaderModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
