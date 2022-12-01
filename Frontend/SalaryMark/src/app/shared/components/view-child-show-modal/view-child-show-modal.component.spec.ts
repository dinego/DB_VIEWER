import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { ViewChildShowModalComponent } from './view-child-show-modal.component';

describe('ViewChildShowModalComponent', () => {
  let component: ViewChildShowModalComponent;
  let fixture: ComponentFixture<ViewChildShowModalComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ ViewChildShowModalComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ViewChildShowModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
