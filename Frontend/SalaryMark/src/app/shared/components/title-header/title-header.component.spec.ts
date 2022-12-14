import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { TitleHeaderComponent } from './title-header.component';

describe('TitleHeaderComponent', () => {
  let component: TitleHeaderComponent;
  let fixture: ComponentFixture<TitleHeaderComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ TitleHeaderComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TitleHeaderComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
