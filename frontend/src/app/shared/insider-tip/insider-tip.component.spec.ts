import { ComponentFixture, TestBed } from '@angular/core/testing';
import { InsiderTipComponent } from './insider-tip.component';

describe('InsiderTipComponent', () => {
  let fixture: ComponentFixture<InsiderTipComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [InsiderTipComponent]
    }).compileComponents();

    fixture = TestBed.createComponent(InsiderTipComponent);
    fixture.componentRef.setInput('title', 'Insider Tip');
    fixture.componentRef.setInput('tip', 'Check your library card perks.');
    fixture.detectChanges();
  });

  it('renders the title and tip', () => {
    const text = fixture.nativeElement.textContent as string;
    expect(text).toContain('Insider Tip');
    expect(text).toContain('Check your library card perks.');
  });
});
