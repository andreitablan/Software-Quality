import {
  AfterViewInit,
  Component,
  OnDestroy,
  ViewEncapsulation,
} from '@angular/core';
import Keyboard from 'simple-keyboard';
import { ApiService } from './api.service';
import { AppSignalRService } from './app-signalR.service';
import { CommonModule } from '@angular/common';

@Component({
  standalone: true,
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
  imports: [CommonModule],
})
export class AppComponent implements AfterViewInit {
  value = '';
  keyboard!: Keyboard;
  receivedMessage: string = '';
  memory: any;

  constructor(
    private apiService: ApiService,
    private signalRService: AppSignalRService
  ) {}

  ngOnInit(): void {
    this.signalRService.startConnection().subscribe(() => {
      this.signalRService.receiveMessage().subscribe((message) => {
        this.receivedMessage = message;
      });

      this.signalRService
        .receiveExecutionResult()
        .subscribe((executionResult) => {
          this.receivedMessage += '\n';

          this.receivedMessage += `Register1: ${executionResult?.registers.reg1}\n`;
          this.receivedMessage += `Register2: ${executionResult?.registers.reg2}\n`;
          this.receivedMessage += `Register3: ${executionResult?.registers.reg3}\n`;
          this.receivedMessage += `Register4: ${executionResult?.registers.reg4}\n`;
          this.receivedMessage += `Register5: ${executionResult?.registers.reg5}\n`;
          this.receivedMessage += `Register6: ${executionResult?.registers.reg6}\n`;
          this.receivedMessage += `Register7: ${executionResult?.registers.reg7}\n`;
          this.receivedMessage += `Register8: ${executionResult?.registers.reg8}\n`;

          this.memory = executionResult.memory;
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
    this.signalRService.sendWipeVideoMemory();
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

  execute() {
    this.apiService.execute().subscribe((x) => {
      console.log('execution');
    });
  }
}
