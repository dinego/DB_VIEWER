import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ColoredTableComponent } from './colored-table.component';

describe('ColoredTableComponent', () => {
  let component: ColoredTableComponent;
  let fixture: ComponentFixture<ColoredTableComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ColoredTableComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ColoredTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
