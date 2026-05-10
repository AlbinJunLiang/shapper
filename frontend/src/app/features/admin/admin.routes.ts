// features/admin/admin.routes.ts
import { Routes } from '@angular/router';
import { Admin } from './admin';
import { StoreInfo } from './store-info/store-info';
import { AdminDashboard } from './admin-dashboard/admin-dashboard';
import { LinkForm } from './link-form/link-form';
import { AdminOrder } from './admin-order/admin-order';
import { OrderDetailTable } from './order-detail-table/order-detail-table';
import { LocationTable } from './location-table/location-table';
import { UserTable } from './user-table/user-table';
import { CategoryTable } from './category-table/category-table';
import { SubcategoryTable } from './subcategory-table/subcategory-table';
import { ProductTable } from './product-table/product-table';
import { FeaturedTable } from './featured-table/featured-table';
import { OrderPaymentTable } from './order-payment-table/order-payment-table';
import { FaqsTable } from './faqs-table/faqs-table';

export const ADMIN_ROUTES: Routes = [
    {
        path: '',
        component: Admin,
        children: [
            { path: '', component: AdminDashboard },
            { path: 'store-info', component: StoreInfo },
            { path: 'links', component: LinkForm },
            { path: 'orders', component: AdminOrder },
            { path: 'orders/:reference', component: OrderDetailTable },
            { path: 'locations', component: LocationTable },
            { path: 'users', component: UserTable },
            { path: 'categories', component: CategoryTable },
            { path: 'subcategories', component: SubcategoryTable },
            { path: 'products', component: ProductTable },
            { path: 'featured', component: FeaturedTable },
            { path: 'order-payments', component: OrderPaymentTable },
            { path: 'faqs', component: FaqsTable },
        ]
    }
];