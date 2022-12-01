import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { ShareHeaderComponent } from './share-header.component';

describe('ShareHeaderComponent', () => {
  let component: ShareHeaderComponent;
  let fixture: ComponentFixture<ShareHeaderComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ ShareHeaderComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ShareHeaderComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
