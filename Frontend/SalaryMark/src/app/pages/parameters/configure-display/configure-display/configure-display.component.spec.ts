import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ConfigureDisplayComponent } from './configure-display.component';

describe('ConfigureDisplayComponent', () => {
  let component: ConfigureDisplayComponent;
  let fixture: ComponentFixture<ConfigureDisplayComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ConfigureDisplayComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ConfigureDisplayComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
