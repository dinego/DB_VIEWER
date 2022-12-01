import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ButtonListVisualizationComponent } from './button-list-visualization.component';

describe('ButtonListVisualizationComponent', () => {
  let component: ButtonListVisualizationComponent;
  let fixture: ComponentFixture<ButtonListVisualizationComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ButtonListVisualizationComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ButtonListVisualizationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
