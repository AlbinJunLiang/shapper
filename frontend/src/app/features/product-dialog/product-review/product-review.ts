import { CommonModule } from '@angular/common';
import { Component, computed, inject, input, signal } from '@angular/core';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIcon } from "@angular/material/icon";
import { FormsModule } from '@angular/forms';
import { ReviewStore } from '../../../core/services/review-store';
import { UserStore } from '../../../core/services/user-store';
import { Review } from '../../../core/interfaces/review.interface';
import { FilterReviewResponse } from '../../../core/interfaces/filter-review-response.interface';
import { REVIEW_CONFIG } from '../review-config';
import { MatProgressSpinner } from "@angular/material/progress-spinner";
import { MatDialog } from '@angular/material/dialog';
import { AuthService } from '../../../core/auth/services/auth-service';
import { NotificationService } from '../../../core/services/notification.service';
import { TranslateModule, TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-product-review',
  imports: [MatIcon, CommonModule, MatFormFieldModule, FormsModule, MatProgressSpinner, TranslateModule],
  templateUrl: './product-review.html',
  styleUrl: './product-review.css',
})
export class ProductReview {

  protected showAllReviews = false;
  protected writeReviewEnabled = false;
  public productId = input<number>();
  protected reviewStore = inject(ReviewStore);
  protected userStore = inject(UserStore);
  protected authService = inject(AuthService);
  protected notify = inject(NotificationService);

  protected readonly REVIEW_CONFIG = REVIEW_CONFIG;
  public reviewsData = input.required<FilterReviewResponse>();
  readonly dialog = inject(MatDialog);

  protected reviewsList = computed(() => this.reviewsData().reviews.data);
  protected currentUserId = computed(() => Number(this.userStore.userSyncData()?.data.id));
  protected reviewsd = this.reviewStore.reviews();
  protected rating: number = 0;
  protected reviewText: string = '';
  protected sortFilter = signal('all');
  protected selectedReviewId = signal(0);
  private translate = inject(TranslateService);



  setRating(value: number) {
    this.rating = value;
  }


  get ratingLabel(): string {
    if (!this.rating || this.rating === 0) {
      return this.translate.instant('REVIEWS.SELECT_RATING');
    }
    return this.translate.instant(`REVIEWS.RATING.${this.rating}`);
  }
  filterByMyReviews() {
    this.reviewStore.reloadReviews(
      Number(this.productId()),
      REVIEW_CONFIG.PAGE_INITIAL,
      1,
      this.currentUserId()
    );
  }


  editReview(reviewId: number) {
    this.selectedReviewId.set(reviewId);
    this.toggleReviewFrom();
  }


  submit() {
    const reviewData: Review = {
      productId: Number(this.productId()),
      userId: Number(this.currentUserId()),
      rating: this.rating,
      comment: this.reviewText
    };

    if (this.reviewStore.hasReview()) {
      this.reviewStore.updateReview(this.selectedReviewId(), reviewData);
    } else {
      this.reviewStore.createReview(reviewData);
    }
    this.cancel();
  }

  isTextInvalid(): boolean {
    const len = this.reviewText.trim().length;
    return len > 0 && (len < 3 || len > 300);
  }

  isRatingMissing(): boolean {
    return this.rating === 0;
  }

  cancel() {
    this.writeReviewEnabled = false;
    this.rating = 0;
    this.reviewText = '';
  }

  // El valor que viene de tu API o base de datos
  getStarFill(starIndex: number): string {
    const rating = this.reviewsData().productStats.averageRating || 0;
    if (rating >= starIndex) return '100%';
    if (rating <= starIndex - 1) return '0%';
    const percentage = (rating - (starIndex - 1)) * 100;
    return `${percentage}%`;
  }

  getBarPercentage(starLevel: number): number {
    const stats = this.reviewsData().productStats.ratingStats;
    const total = this.reviewsData().productStats.totalReviews;

    if (!stats || total === 0) return 0;

    // Buscamos el conteo para esta estrella específica
    const starData = stats.find(s => s.stars === starLevel);

    // Calculamos el porcentaje: (cantidad / total) * 100
    return starData ? (starData.count / total) * 100 : 0;
  }

  getStarCount(starLevel: number): number {
    const stats = this.reviewsData().productStats.ratingStats;
    return stats.find(s => s.stars === starLevel)?.count ?? 0;
  }



  async toggleReviewFrom() {
    if (!this.authService.isLoggedIn()) {
      this.notify.show("Inicia sesión para comentar.");
      return;
    }
    this.writeReviewEnabled = !this.writeReviewEnabled;

    if (this.writeReviewEnabled) {
      const userId = this.currentUserId(); // <--- Ahora es una función ()
      const prodId = this.productId();

      if (userId && prodId) {
        const existingReview = await this.reviewStore.checkUserReviewStatusAsync(Number(prodId), userId);

        if (existingReview) {
          this.reviewText = existingReview.comment;
          this.rating = existingReview.rating;
        }
      }
    } else {
      this.cancel();
    }
  }


  filterSortBy(sortBy: string) {
    this.sortFilter.set(sortBy);
    this.showAllReviews = true; // Abrimos la lista para ver los resultados

    this.reviewStore.reloadReviews(
      Number(this.productId()),
      1,
      REVIEW_CONFIG.PAGE_SIZE,
      undefined,
      sortBy
    );
  }

  loadMore() {
    const pagination = this.reviewStore.pagination();

    // Caso A: La lista está colapsada (solo muestra 3 o 5)
    if (!this.showAllReviews) {
      this.showAllReviews = true;

      // Si solo tenemos la página 1 y el servidor dice que hay más, traemos la 2 de una vez
      if (pagination.current === 1 && pagination.total > 1) {
        this.fetchNextPage();
      }
      return;
    }

    // Caso B: La lista ya está expandida y queremos más datos del servidor
    if (pagination.current < pagination.total) {
      this.fetchNextPage();
    }
    // Caso C: Ya no hay más que cargar, el usuario quiere encoger la lista
    else {
      this.showAllReviews = false;
    }
  }


  async deleteReview(reviewId: number) {
    await this.reviewStore.deleteReview(reviewId);
  }
  private fetchNextPage() {
    const pagination = this.reviewStore.pagination();
    this.reviewStore.loadReviews(
      Number(this.productId()),
      pagination.current + 1,
      REVIEW_CONFIG.PAGE_SIZE,
      undefined, // O this.currentUserId si quieres paginar sobre "Mis reseñas"
      this.sortFilter() // <--- USAMOS EL SIGNAL AQUÍ
    );
  }

  protected toNumber(value: string) {
    return Number(value);
  }
}
