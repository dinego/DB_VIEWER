import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DropdownParametersComponent } from './dropdown-parameters.component';

describe('DropdownParametersComponent', () => {
  let component: DropdownParametersComponent;
  let fixture: ComponentFixture<DropdownParametersComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DropdownParametersComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DropdownParametersComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
