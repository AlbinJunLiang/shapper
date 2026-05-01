import { Injectable, signal, computed, OnDestroy } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class TimerService implements OnDestroy {
  private readonly STORAGE_KEY = 'shapper_forgot_timeout';
  private readonly COOLDOWN_MINUTES = 10;
  private timerInterval: any;
  public countdown = signal<string>('10:00');
  public isTimerActive = signal<boolean>(false);
  public itWasSent = computed(() => this.isTimerActive());

  constructor() {
    this.checkExistingTimeout();
  }

  ngOnDestroy() {
    this.stopInterval();
  }

  start() {
    const targetTime = Date.now() + this.COOLDOWN_MINUTES * 60 * 1000;
    localStorage.setItem(this.STORAGE_KEY, targetTime.toString());
    this.runTimer(targetTime);
  }

  private runTimer(targetTime: number) {
    this.isTimerActive.set(true);
    this.stopInterval();

    const update = () => {
      const now = Date.now();
      const diff = targetTime - now;

      if (diff <= 0) {
        this.clearTimeout();
        return;
      }

      const m = Math.floor(diff / 60000);
      const s = Math.floor((diff % 60000) / 1000);
      this.countdown.set(`${m}:${s.toString().padStart(2, '0')}`);
    };

    update();
    this.timerInterval = setInterval(update, 1000);
  }

  private checkExistingTimeout() {
    const savedTarget = localStorage.getItem(this.STORAGE_KEY);
    console.log('Revisando storage:', savedTarget);

    if (savedTarget) {
      const targetTime = parseInt(savedTarget, 10);
      const now = Date.now();

      console.log('Diferencia de tiempo:', targetTime - now);

      if (targetTime > now) {
        this.runTimer(targetTime);
      } else {
        console.log('Tiempo expirado, limpiando...');
        this.clearTimeout();
      }
    }
  }

  private clearTimeout() {
    this.isTimerActive.set(false);
    localStorage.removeItem(this.STORAGE_KEY);
    this.stopInterval();
  }

  private stopInterval() {
    if (this.timerInterval) clearInterval(this.timerInterval);
  }
}