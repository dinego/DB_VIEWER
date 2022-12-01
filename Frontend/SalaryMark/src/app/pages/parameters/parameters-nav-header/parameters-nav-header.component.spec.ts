import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { ParametersNavHeaderComponent } from './parameters-nav-header.component';

describe('ParametersNavHeaderComponent', () => {
  let component: ParametersNavHeaderComponent;
  let fixture: ComponentFixture<ParametersNavHeaderComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ParametersNavHeaderComponent],
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ParametersNavHeaderComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
