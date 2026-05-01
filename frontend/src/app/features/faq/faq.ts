import { Component, inject, signal } from '@angular/core';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatIcon } from "@angular/material/icon";
import { FaqStore } from '../../core/services/faq.sore';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-faq',
  standalone: true,
  imports: [MatExpansionModule, MatIcon, TranslateModule],
  templateUrl: './faq.html',
  styleUrl: './faq.css',
})
export class Faq {
  protected faqStore = inject(FaqStore);

  ngOnInit() {
    this.faqStore.loadFaqs(1, 100);
  }
}