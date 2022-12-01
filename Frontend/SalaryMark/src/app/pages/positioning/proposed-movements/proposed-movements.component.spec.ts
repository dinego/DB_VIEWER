import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { ProposedMovementsComponent } from './proposed-movements.component';

describe('ProposedMovementsComponent', () => {
  let component: ProposedMovementsComponent;
  let fixture: ComponentFixture<ProposedMovementsComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ ProposedMovementsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ProposedMovementsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
