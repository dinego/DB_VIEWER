import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ItemButtonRemoveComponent } from './item-button-remove.component';

describe('ItemButtonRemoveComponent', () => {
  let component: ItemButtonRemoveComponent;
  let fixture: ComponentFixture<ItemButtonRemoveComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ItemButtonRemoveComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ItemButtonRemoveComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
