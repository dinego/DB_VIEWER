import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { SendLinkAccessComponent } from './send-link-access.component';

describe('SendLinkAccessComponent', () => {
  let component: SendLinkAccessComponent;
  let fixture: ComponentFixture<SendLinkAccessComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ SendLinkAccessComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SendLinkAccessComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
