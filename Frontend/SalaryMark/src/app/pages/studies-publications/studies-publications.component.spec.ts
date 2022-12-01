import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StudiesPublicationsComponent } from './studies-publications.component';

describe('StudiesPublicationsComponent', () => {
  let component: StudiesPublicationsComponent;
  let fixture: ComponentFixture<StudiesPublicationsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ StudiesPublicationsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(StudiesPublicationsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
