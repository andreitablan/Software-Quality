import {
  AfterViewInit,
  Component,
  OnDestroy,
  ViewEncapsulation,
} from '@angular/core';
import Keyboard from 'simple-keyboard';
import { ApiService } from './api.service';
import { AppSignalRService } from './app-signalR.service';

@Component({
  standalone: true,
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent implements AfterViewInit {
  value = '';
  keyboard!: Keyboard;
  receivedMessage: string = '';

  constructor(
    private apiService: ApiService,
    private signalRService: AppSignalRService
  ) {}

  ngOnInit(): void {
    this.signalRService.startConnection().subscribe(() => {
      this.signalRService.receiveMessage().subscribe((message) => {
        this.receivedMessage = message;
      });
    });

    window.addEventListener('unload', () => {
      this.sendWipeVideoMemory();
    });
  }

  sendMessage(message: string): void {
    this.signalRService.sendMessage(message);
  }

  sendBackspace(): void {
    this.signalRService.sendBackspace();
  }

  sendWipeVideoMemory(): void {
    for (let i = 0; i < this.receivedMessage.length; i++) {
      this.sendBackspace();
    }
  }

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
    switch (button) {
      case '{enter}':
        this.handleEnter();
        break;
      case '{lock}':
      case '{shift}':
        this.handleShift();
        break;
      case '{tab}':
        this.sendMessage('  ');
        break;
      case '{space}':
        this.sendMessage(' ');
        break;
      case '{bksp}':
        this.sendBackspace();
        break;
      case '.com':
        for (let i = 0; i < button.length; i++) {
          this.sendMessage(button[i]);
        }
        break;
      default:
        this.sendMessage(button);
    }
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
    this.keyboard.setOptions({
      layoutName: 'default',
    });
    this.sendMessage('\n');
  };
}
