import { AfterViewInit, Component, ViewEncapsulation } from '@angular/core';
import Keyboard from 'simple-keyboard';
import { ApiService } from './api.service';

@Component({
  standalone: true,
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent implements AfterViewInit {
  value = '';
  keyboard!: Keyboard;

  constructor(private apiService: ApiService) {}

  ngAfterViewInit() {
    const textarea = document.getElementById('myTextarea');
    textarea?.focus();
    if (!textarea) return;
    // Listen for the blur event
    textarea.addEventListener('blur', function () {
      // Set focus back to the textarea
      textarea.focus();
    });

    this.keyboard = new Keyboard({
      onChange: (input) => this.onChange(input),
      onKeyPress: (button) => this.onKeyPress(button),
      enterKeyHint: () => this.handleEnter(),
    });
  }

  onChange = (input: string) => {
    this.value = input;
  };

  onKeyPress = (button: string) => {
    this.apiService.sendLetter(button);

    if (button === '{enter}') this.handleEnter();
    if (button === '{shift}') this.handleShift();
    if (button === '{lock}') this.handleLock();
  };

  onInputChange = (event: any) => {
    this.keyboard.setInput(event.target.value);
  };

  handleLock = () => {};

  handleShift = () => {
    let currentLayout = this.keyboard.options.layoutName;
    let shiftToggle = currentLayout === 'default' ? 'shift' : 'default';

    this.keyboard.setOptions({
      layoutName: shiftToggle,
    });
  };

  handleEnter = () => {
    this.value = this.value + '\n';

    this.keyboard.setOptions({
      layoutName: 'default',
    });

    this.keyboard.setInput(this.value);
  };
}
