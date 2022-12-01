import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ApplyUpdateComponent } from './apply-update.component';

describe('ApplyUpdateComponent', () => {
  let component: ApplyUpdateComponent;
  let fixture: ComponentFixture<ApplyUpdateComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ApplyUpdateComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ApplyUpdateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
