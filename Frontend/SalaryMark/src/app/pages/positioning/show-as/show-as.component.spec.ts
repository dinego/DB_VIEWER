import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';
import { ShowAsComponent } from './show-as.component';

describe('ShowAsComponent', () => {
  let component: ShowAsComponent;
  let fixture: ComponentFixture<ShowAsComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ShowAsComponent],
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ShowAsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
