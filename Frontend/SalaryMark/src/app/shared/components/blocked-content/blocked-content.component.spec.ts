import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BlockedContentComponent } from './blocked-content.component';

describe('BlockedContentComponent', () => {
  let component: BlockedContentComponent;
  let fixture: ComponentFixture<BlockedContentComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [BlockedContentComponent],
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(BlockedContentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
