import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { PJSettingsComponent } from './pj-settings.component';

describe('PJSettingsComponent', () => {
  let component: PJSettingsComponent;
  let fixture: ComponentFixture<PJSettingsComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [PJSettingsComponent],
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PJSettingsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
