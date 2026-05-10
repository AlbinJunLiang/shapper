import { Routes } from '@angular/router';
import { Order } from './features/order/order';
import { ProductList } from './features/product-list/product-list';
import { Home } from './features/home/home';
import { DetailsDialogWrapper } from './features/details-dialog-wrapper/details-dialog-wrapper';
import { UserInfo } from './features/user-info/user-info';
import { authGuard } from './core/guards/auth.guard';
import { Success } from './features/success/success';
import { Faq } from './features/faq/faq';
import { adminGuard } from './core/guards/admin.guards';

export const routes: Routes = [
    { path: '', redirectTo: 'home', pathMatch: 'full' },
    { path: 'home', component: Home },

    // Lazy Loaded Admin Section
    {
        path: 'admin',
        canActivate: [adminGuard],
        loadChildren: () => import('./features/admin/admin.routes').then(m => m.ADMIN_ROUTES)
    },

    { path: 'success', component: Success },
    { path: 'faqs', component: Faq },
    { path: 'orders', component: Order, canActivate: [authGuard] },
    { path: 'user-information', component: UserInfo, canActivate: [authGuard] },
    {
        path: 'products',
        component: ProductList,
        children: [
            { path: 'details/:id', component: DetailsDialogWrapper }
        ]
    },
    { path: 'products/:filter', component: ProductList },
    { path: '**', redirectTo: 'home' },
];