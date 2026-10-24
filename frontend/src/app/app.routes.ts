import { Routes } from '@angular/router';
import { Login } from './components/login/login';
import { Inventory } from './components/inventory/inventory';

export const routes: Routes = 
[
 { path: 'login', component: Login },
 { path: 'inventory', component: Inventory },
 { path: '', redirectTo: 'login', pathMatch: 'full' },
 { path: '**', redirectTo: 'login' }
];
