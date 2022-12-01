import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { CheckeboxShowDialogComponent } from './checkebox-show-dialog.component';

describe('CheckeboxShowDialogComponent', () => {
  let component: CheckeboxShowDialogComponent;
  let fixture: ComponentFixture<CheckeboxShowDialogComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ CheckeboxShowDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CheckeboxShowDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
