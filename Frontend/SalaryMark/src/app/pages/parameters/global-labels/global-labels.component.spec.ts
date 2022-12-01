import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GlobalLabelsComponent } from './global-labels.component';

describe('GlobalLabelsComponent', () => {
  let component: GlobalLabelsComponent;
  let fixture: ComponentFixture<GlobalLabelsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ GlobalLabelsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(GlobalLabelsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
