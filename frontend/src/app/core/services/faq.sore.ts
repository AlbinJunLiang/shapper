import { inject, Injectable, signal, computed } from '@angular/core';
import { finalize, tap } from 'rxjs';
import { FaqService } from '../services/faq.service';
import { IFaq, CreateFaq, UpdateFaq } from '../interfaces/faq.interface';
import { PagedFaq } from '../interfaces/paged-faq.interface';

@Injectable({ providedIn: 'root' })
export class FaqStore {
    private faqService = inject(FaqService);

    private _faqs = signal<IFaq[]>([]);
    private _loading = signal<boolean>(false);
    private _totalFaqs = signal<number>(0);

    public faqs = computed(() => this._faqs());
    public isLoading = computed(() => this._loading());
    public totalFaqs = computed(() => this._totalFaqs());

    /**
     * Carga inicial o scroll infinito (No sobreescribe, mezcla)
     */
    loadFaqs(page: number = 1, pageSize: number = 10) {
        if (this._loading()) return;
        this._loading.set(true);

        this.faqService.getFaqs(page, pageSize).subscribe({
            next: (response: PagedFaq) => {
                this.updateFaqsState(response.data);
                this._totalFaqs.set(response.totalCount);
                this._loading.set(false);
            },
            error: () => this._loading.set(false)
        });
    }

    /**
     * Obtiene FAQs limpiando el estado anterior (Útil para filtros o refrescar)
     */
    getFaqs(page: number = 1, pageSize: number = 10) {
        this._loading.set(true);

        this.faqService.getFaqs(page, pageSize).subscribe({
            next: (response: PagedFaq) => {
                this._faqs.set(response.data);
                this._totalFaqs.set(response.totalCount);
                this._loading.set(false);
            },
            error: () => this._loading.set(false)
        });
    }

    /**
     * Crear FAQ y actualizar estado local
     */
    createFaq(data: CreateFaq) {
        this._loading.set(true);
        return this.faqService.create(data).pipe(
            tap((newFaq: IFaq) => {
                this.getFaqs();

            }),
            finalize(() => this._loading.set(false))
        );
    }

    /**
     * Actualizar FAQ y refrescar item específico en el array
     */
    updateFaq(id: number, data: UpdateFaq) {
        this._loading.set(true);

        return this.faqService.update(id, data).pipe(
            tap(() => {
                // OPCIÓN SIMPLE: Olvida el mapeo manual. 
                // Refresca toda la lista para asegurar que los datos sean 100% reales.
                this.getFaqs();
            }),
            finalize(() => this._loading.set(false))
        );
    }
    /**
     * Eliminar FAQ y limpiar estado local
     */
    deleteFaq(id: number) {
        this._loading.set(true);
        return this.faqService.delete(id).pipe(
            tap(() => {
                this._faqs.update(current => current.filter(f => f.id !== id));
                this._totalFaqs.update(total => total - 1);
            }),
            finalize(() => this._loading.set(false))
        );
    }

    /**
     * LÓGICA DE MEZCLA (Merge)
     * Evita duplicados al cargar más páginas
     */
    private updateFaqsState(newFaqs: IFaq[]) {
        this._faqs.update(prev => {
            const uniqueNew = newFaqs.filter(
                nFaq => !prev.some(oldFaq => oldFaq.id === nFaq.id)
            );
            return [...prev, ...uniqueNew];
        });
    }
}