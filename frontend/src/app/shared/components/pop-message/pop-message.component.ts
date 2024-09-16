import { Component, Input, Renderer2, OnInit } from '@angular/core';

@Component({
  selector: 'app-pop-message',
  templateUrl: './pop-message.component.html',
  styleUrls: ['./pop-message.component.css'],
})
export class PopMessageComponent implements OnInit {
  @Input() message: string = ''; // The message to be displayed
  isVisible: boolean = false;

  constructor(private renderer: Renderer2) {}

  ngOnInit(): void {
    this.showMessage();
  }

  showMessage() {
    this.isVisible = true;
    setTimeout(() => {
      this.renderer.addClass(
        document.querySelector('.pop-message'),
        'fade-out'
      );
      setTimeout(() => {
        this.isVisible = false;
      }, 1000); // Duration for fade-out effect
    }, 7000); // Message stays for 7 seconds
  }
}
