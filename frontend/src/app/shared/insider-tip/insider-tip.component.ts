import { Component, Input } from '@angular/core';

@Component({
  selector: 'qi-insider-tip',
  standalone: true,
  template: `
    <aside class="insider-tip">
      <strong>⚜ {{ title }}</strong>
      <p>{{ tip }}</p>
    </aside>
  `
})
export class InsiderTipComponent {
  @Input({ required: true }) title = '';
  @Input({ required: true }) tip = '';
}
