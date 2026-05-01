import { inject, Injectable, signal, computed } from '@angular/core';
import { ReviewService } from './review-service'; // Tu servicio de Angular
import { FilterReviewResponse } from '../interfaces/filter-review-response.interface';
import { ReviewResponse } from '../interfaces/review-response.interface';
import { Review } from '../interfaces/review.interface';
import { finalize, firstValueFrom } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class ReviewStore {
    private reviewService = inject(ReviewService);

    private _apiResponse = signal<FilterReviewResponse>({
        reviews: {
            totalCount: 0,
            totalPages: 0,
            page: 1,
            pageSize: 10,
            data: []
        },
        productStats: {
            averageRating: 0,
            totalReviews: 0,
            ratingStats: []
        }
    });


    public apiResponse = this._apiResponse.asReadonly();

    public reviews = computed(() => this._apiResponse().reviews?.data ?? []);
    public pagination = computed(() => ({
        current: this._apiResponse().reviews?.page ?? 1,
        total: this._apiResponse().reviews?.totalPages ?? 0,
        totalItems: this._apiResponse().reviews?.totalCount ?? 0
    }));
    public stats = computed(() => this._apiResponse().productStats);

    private _currentUserReview = signal<Review | null>(null);
    public currentUserReview = this._currentUserReview.asReadonly();
    public hasReview = computed(() => this._currentUserReview() !== null);
    private _isCheckingAccount = signal<boolean>(false); // Nuevo signal
    public isCheckingAccount = this._isCheckingAccount.asReadonly();


    private _loading = signal<boolean>(false);
    public isLoading = this._loading.asReadonly();




    /**
     * Carga de reseñas con filtros y paginación
     */
    loadReviews(productId: number, page: number = 1, pageSize: number = 10, userId?: number, sortBy?: string) {
        if (this._loading()) return;
        this._loading.set(true);

        this.reviewService.getFilteredReviews(productId, userId, sortBy, page, pageSize).subscribe({
            next: (response) => {
                this._apiResponse.update(state => {
                    const newData = page === 1
                        ? response.reviews.data
                        : [...state.reviews.data, ...response.reviews.data];

                    return {
                        ...response,
                        reviews: {
                            ...response.reviews,
                            data: newData
                        }
                    };
                });
            },
            error: () => this._loading.set(false),
            complete: () => this._loading.set(false)
        });
    }


    /**
 * Crea una nueva reseña y actualiza el estado
 */

    createReview(review: Review) {
        if (this._loading()) return;
        this._loading.set(true);

        this.reviewService.createReview(review)
            .pipe(
                finalize(() => this._loading.set(false)) // Se ejecuta al terminar, sea éxito o error
            )
            .subscribe({
                // En tu método createReview, después de addLocalReview:
                next: (newReview) => {
                    this.addLocalReview(newReview);
                    // Sincronizamos el estado del usuario actual
                    this._currentUserReview.set({
                        productId: Number(newReview.productId),
                        userId: newReview.userId,
                        rating: newReview.rating,
                        comment: newReview.comment
                    });
                },
                error: (err) => {
                    console.error('Error creating review:', err);
                },
            });
    }
    /**
     * Agregar una nueva reseña localmente (Optimistic Update)
     * Útil para mostrar la reseña recién creada sin recargar toda la API
     */

    addLocalReview(newReview: ReviewResponse) {
        this._apiResponse.update(state => {
            // Evitar duplicados por ID
            const exists = state.reviews.data.some(r => r.id === newReview.id);
            if (exists) return state;

            return {
                ...state,
                reviews: {
                    ...state.reviews,
                    data: [newReview, ...state.reviews.data],
                    totalCount: state.reviews.totalCount + 1
                }
            };
        });
    }

    /**
 * Limpia el estado actual y carga datos nuevos desde cero
 */
    reloadReviews(productId: number, page: number = 1, pageSize: number = 10, userId?: number, sortBy?: string) {
        this._loading.set(false);
        this.clearStore();
        this.loadReviews(productId, page, pageSize, userId, sortBy);
    }

    clearStore() {
        this._apiResponse.set({
            reviews: { totalCount: 0, totalPages: 0, page: 1, pageSize: 10, data: [] },
            productStats: { averageRating: 0, totalReviews: 0, ratingStats: [] }
        });
    }


    // En ReviewStore
    async checkUserReviewStatusAsync(productId: number, userId: number): Promise<Review | null> {
        this._isCheckingAccount.set(true);

        try {
            // Convertimos el Observable a Promesa
            const response = await firstValueFrom(
                this.reviewService.getFilteredReviews(productId, userId, '', 1, 1)
            );

            const review = response.reviews.data[0] || null;

            // Mapeamos y actualizamos el signal del Store
            const mappedReview = review ? {
                productId: review.productId,
                userId: Number(review.userId),
                rating: review.rating,
                comment: review.comment
            } : null;

            this._currentUserReview.set(mappedReview);
            return mappedReview;

        } catch (error) {
            this._currentUserReview.set(null);
            return null;
        } finally {
            this._isCheckingAccount.set(false);
        }
    }

    // Dentro de tu ReviewStore
    async deleteReview(reviewId: number) {
        this._loading.set(true);
        try {
            await firstValueFrom(this.reviewService.deleteReview(reviewId));

            // ACTUALIZACIÓN CORRECTA: Modificamos el estado base (_apiResponse)
            this._apiResponse.update(state => ({
                ...state,
                reviews: {
                    ...state.reviews,
                    // Filtramos la data dentro del objeto de respuesta
                    data: state.reviews.data.filter(r => r.id !== reviewId),
                    totalCount: Math.max(0, state.reviews.totalCount - 1)
                }
            }));

            // Limpiamos la reseña del usuario actual en el estado local
            this._currentUserReview.set(null);

        } catch (error) {
            console.error('Error al eliminar:', error);
        } finally {
            this._loading.set(false);
        }
    }
    async updateReview(id: number, review: Review) {
        if (this._loading()) return;
        this._loading.set(true);

        try {
            const response = await firstValueFrom(this.reviewService.updateReview(id, review));

            const updatedReview = response.data;

            this._apiResponse.update(state => ({
                ...state,
                reviews: {
                    ...state.reviews,
                    data: state.reviews.data.map(r =>
                        r.id === id
                            ? { ...r, ...updatedReview } // Mantenemos Name/LastName de 'r'
                            : r
                    )
                }
            }));

            this._currentUserReview.set({
                productId: updatedReview.productId,
                userId: Number(updatedReview.userId),
                rating: updatedReview.rating,
                comment: updatedReview.comment
            });

        } catch (error) {
            console.error('Error al actualizar:', error);
        } finally {
            this._loading.set(false);
        }
    }

}