import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LobbuTabComponent } from './lobbu-tab.component';

describe('LobbuTabComponent', () => {
  let component: LobbuTabComponent;
  let fixture: ComponentFixture<LobbuTabComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ LobbuTabComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(LobbuTabComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
