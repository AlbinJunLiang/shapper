import { Component, inject, signal, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { AuthService } from '../../services/auth-service';
import { switchMap, take, throwError } from 'rxjs';
import { NotificationService } from '../../../services/notification.service';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-verify-dialog',
  standalone: true,
  imports: [CommonModule, MatIconModule, MatDialogModule, TranslateModule],
  templateUrl: './verify-dialog.html'
})
export class VerifyDialog implements OnInit, OnDestroy { 

  step = signal<'initial' | 'sent'>('initial');
   countdown = signal<string>('10:00');
  private authService = inject(AuthService);
  private timerInterval: any;
  private readonly STORAGE_KEY = 'shapper_mail_timeout';
  private readonly COOLDOWN_MINUTES = 10;
  private notify = inject(NotificationService);
  private dialogRef = inject(MatDialogRef<VerifyDialog>);


  ngOnInit() {
    this.checkExistingTimeout();
  }


  ngOnDestroy() {
    if (this.timerInterval) clearInterval(this.timerInterval);
  }

  private checkExistingTimeout() {
    const savedTarget = localStorage.getItem(this.STORAGE_KEY);

    if (savedTarget) {
      const targetTime = parseInt(savedTarget, 10);
      const now = Date.now();

      if (targetTime > now) {
        this.step.set('sent');
        this.startTimer(targetTime);
      } else {
        this.clearTimeout();
      }
    }
  }


  sendVerification() {
    this.authService.refreshAuthStatus().pipe(
      take(1),
      switchMap((isVerified) => {
        if (isVerified) {
          return throwError(() => 'USER_ALREADY_VERIFIED');
        }
        return this.authService.sendEmailVerification().pipe(take(1));
      })
    ).subscribe({
      next: () => {
        const targetTime = Date.now() + this.COOLDOWN_MINUTES * 60 * 1000;
        localStorage.setItem(this.STORAGE_KEY, targetTime.toString());
        this.step.set('sent');
        this.startTimer(targetTime);
      },
      error: (err) => {
        if (err === 'USER_ALREADY_VERIFIED') {
          this.notify.show('El usuario ya se encuentra verificado.');
          this.dialogRef.close();
        } else {
          console.error('Error en el proceso de verificación:', err);
          this.notify.show('Ha ocurrido un error, vuélvalo intentar más tarde.')

        }
      }
    });
  }

  private startTimer(targetTime: number) {
    if (this.timerInterval) clearInterval(this.timerInterval);

    const update = () => {
      const now = Date.now();
      const diff = targetTime - now;

      if (diff <= 0) {
        this.clearTimeout();
        return;
      }

      const minutes = Math.floor(diff / 60000);
      const seconds = Math.floor((diff % 60000) / 1000);
      this.countdown.set(`${minutes}:${seconds.toString().padStart(2, '0')}`);
    };

    update();
    this.timerInterval = setInterval(update, 1000);
  }

  private clearTimeout() {
    this.step.set('initial');
    localStorage.removeItem(this.STORAGE_KEY);
    if (this.timerInterval) clearInterval(this.timerInterval);
  }
}