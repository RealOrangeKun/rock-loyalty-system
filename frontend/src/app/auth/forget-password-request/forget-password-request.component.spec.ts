import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ForgetPasswordRequestComponent } from './forget-password-request.component';

describe('ForgetPasswordRequestComponent', () => {
  let component: ForgetPasswordRequestComponent;
  let fixture: ComponentFixture<ForgetPasswordRequestComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ForgetPasswordRequestComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ForgetPasswordRequestComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
