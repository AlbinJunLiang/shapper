import { Routes } from '@angular/router';
import { Order } from './features/order/order';
import { ProductList } from './features/product-list/product-list';
import { Home } from './features/home/home';
import { DetailsDialogWrapper } from './features/details-dialog-wrapper/details-dialog-wrapper';
import { UserInfo } from './features/user-info/user-info';
import { authGuard } from './core/guards/auth.guard';
import { Admin } from './features/admin/admin';
import { StoreInfo } from './features/admin/store-info/store-info';
import { AdminDashboard } from './features/admin/admin-dashboard/admin-dashboard';
import { LinkForm } from './features/admin/link-form/link-form';
import { AdminOrder } from './features/admin/admin-order/admin-order';
import { OrderDetailTable } from './features/admin/order-detail-table/order-detail-table';
import { LocationTable } from './features/admin/location-table/location-table';
import { UserTable } from './features/admin/user-table/user-table';
import { CategoryTable } from './features/admin/category-table/category-table';
import { SubcategoryTable } from './features/admin/subcategory-table/subcategory-table';
import { ProductTable } from './features/admin/product-table/product-table';
import { Success } from './features/success/success';
import { FeaturedTable } from './features/admin/featured-table/featured-table';
import { OrderPaymentTable } from './features/admin/order-payment-table/order-payment-table';
import { FaqsTable } from './features/admin/faqs-table/faqs-table';
import { Faq } from './features/faq/faq';
import { adminGuard } from './core/guards/admin.guards';

export const routes: Routes = [
    { path: '', redirectTo: 'home', pathMatch: 'full' },
    { path: 'home', component: Home },
    {
        path: 'admin', component: Admin, canActivate: [adminGuard], children: [
            {
                path: 'store-info',
                component: StoreInfo
            },
            {
                path: '',
                component: AdminDashboard
            },
            {
                path: 'links',
                component: LinkForm
            },
            {
                path: 'orders',
                component: AdminOrder
            },
            {
                path: 'orders/:reference',
                component: OrderDetailTable
            },
            {
                path: 'locations',
                component: LocationTable
            },
            {
                path: 'users',
                component: UserTable
            },
            {
                path: 'categories',
                component: CategoryTable
            },
            {
                path: 'subcategories',
                component: SubcategoryTable
            },
            {
                path: 'products',
                component: ProductTable
            },
            {
                path: 'featured',
                component: FeaturedTable
            }
            ,
            {
                path: 'order-payments',
                component: OrderPaymentTable
            },
            {
                path: 'faqs',
                component: FaqsTable
            }
        ]
    },
    { path: 'success', component: Success },
    { path: 'faqs', component: Faq },

    { path: 'orders', component: Order, canActivate: [authGuard] },
    { path: 'user-information', component: UserInfo, canActivate: [authGuard] },
    {
        path: 'products',
        component: ProductList,
        children: [
            {
                path: 'details/:id',
                component: DetailsDialogWrapper
            }
        ]
    },
    { path: 'products/:filter', component: ProductList },
    { path: '**', redirectTo: 'home' },
];