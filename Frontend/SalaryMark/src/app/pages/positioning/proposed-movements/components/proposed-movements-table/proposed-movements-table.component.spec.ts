import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProposedMovementsTableComponent } from './proposed-movements-table.component';

describe('ProposedMovementsTableComponent', () => {
  let component: ProposedMovementsTableComponent;
  let fixture: ComponentFixture<ProposedMovementsTableComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ProposedMovementsTableComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ProposedMovementsTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
