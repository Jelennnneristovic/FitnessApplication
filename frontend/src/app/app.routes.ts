import { Routes } from '@angular/router';
import { Login } from './features/auth/login/login';
import { Register } from './features/auth/register/register';
import { Home } from './features/home/home';
import { AdminDashboard } from './features/admin/admin-dashboard/admin-dashboard';
import { TrainerDashboard } from './features/trainer/trainer-dashboard/trainer-dashboard';
import { ClientDashboard } from './features/client/client-dashboard/client-dashboard';

export const routes: Routes = [
  { path: 'login', component: Login },
  { path: 'register', component: Register },
  { path: 'admin', component: AdminDashboard },
  { path: 'trainer', component: TrainerDashboard },
  { path: 'client', component: ClientDashboard },
  { path: '', component: Home, pathMatch: 'full' },
  { path: '**', redirectTo: '/login' }
];