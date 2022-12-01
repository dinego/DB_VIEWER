import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FilterByHeaderComponent } from './filter-by-header.component';

describe('FilterByHeaderComponent', () => {
  let component: FilterByHeaderComponent;
  let fixture: ComponentFixture<FilterByHeaderComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ FilterByHeaderComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(FilterByHeaderComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
