import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { BasicDialogStructureComponent } from './basic-dialog-structure.component';

describe('BasicDialogStructureComponent', () => {
  let component: BasicDialogStructureComponent;
  let fixture: ComponentFixture<BasicDialogStructureComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ BasicDialogStructureComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(BasicDialogStructureComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
