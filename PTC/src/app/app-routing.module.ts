import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ProductListComponent } from "./product/product-list.component";
import { CategoryListComponent } from './category/category-list.component';
import { DashboardComponent } from './dashboard/dashboard.component';
import { PageNotFoundComponent } from './public/page-not-found.component';
import { ProductDetailComponent } from './product/product-detail.component';
import { ValidIdGuard } from './shared/guards/valid-id.guard';
import { NotSavedGuard } from './shared/guards/not-saved.guard';
import { LogMaintenanceComponent } from './shared/logging/log-maintenance.component';
import { ConfigurationComponent } from './shared/configuration/configuration.component';
import { LoginComponent } from './public/login.component';
import { AuthGuard } from './shared/guards/auth.guard';

const routes: Routes = [
  {
    path: 'dashboard',
    component: DashboardComponent
  },
  {
    path: 'login', 
    component: LoginComponent
  },
  {
    path: 'categories',
    component: CategoryListComponent,
    canActivate: [AuthGuard],
    data: { claimType: 'canAccessCategories' }  
  },
  {
    path: 'settings',
    component: ConfigurationComponent,
    canActivate: [AuthGuard],
    data: { claimType: 'canAccessSettings' }
  },
  {
    path: 'logmaintenance',
    component: LogMaintenanceComponent,
    canActivate: [AuthGuard],
    data: { claimType: 'canAccessLogs' }
  },
  {
    path: 'products',
    component: ProductListComponent,
    canActivate: [AuthGuard],
    data: { claimType: 'canAccessProducts' }
  },
  {
    path: 'productDetail/:id',
    canActivate: [ValidIdGuard, AuthGuard],
    canDeactivate: [NotSavedGuard],
    data: { redirectTo: 'products', claimType: 'canAccessProducts' },
    component: ProductDetailComponent
  },
  {
    path: '', redirectTo: 'dashboard', pathMatch: 'full'
  },
  {
    path: '**', component: PageNotFoundComponent
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
