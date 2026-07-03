import { Routes } from '@angular/router';
import { Login } from './features/auth/login/login';
import { Register } from './features/auth/register/register';
import { Home } from './features/home/home';
import { AdminDashboard } from './features/admin/admin-dashboard/admin-dashboard';
import { TrainerDashboard } from './features/trainer/trainer-dashboard/trainer-dashboard';
import { ClientDashboard } from './features/client/client-dashboard/client-dashboard';
import { authGuard, roleGuard } from './core/guards/auth.guard';
import { UserRole } from './core/models/user.model';
import { ClientsList } from './features/admin/clients-list/clients-list';
import { TrainersList } from './features/admin/trainers-list/trainers-list';
import { CategoriesList } from './features/admin/categories-list/categories-list';
import { TrainerSearch } from './features/client/trainer-search/trainer-search';
import { TrainerDetail } from './features/client/trainer-detail/trainer-detail';
import { MyEnrollments } from './features/client/my-enrollments/my-enrollments';
import { Notifications } from './shared/notifications/notifications';
import { TrainerEnrollments } from './features/trainer/trainer-enrollments/trainer-enrollments';
import { MyPlans } from './features/trainer/my-plans/my-plans';
import { PlanForm } from './features/trainer/plan-form/plan-form';
import { TrainerProfile } from './features/trainer/trainer-profile/trainer-profile';
import { PlanSessions } from './features/trainer/plan-sessions/plan-sessions';
import { SessionAttendance } from './features/trainer/session-attendance/session-attendance';
import { MySchedule } from './features/client/my-schedule/my-schedule';


export const routes: Routes = [
  { path: 'login', component: Login },
  { path: 'register', component: Register },

  {
    path: 'admin',
    component: AdminDashboard,
    canActivate: [roleGuard],
    data: { role: UserRole.Admin }
  },
  {
    path: 'admin/clients',
    component: ClientsList,
    canActivate: [roleGuard],
    data: { role: UserRole.Admin }
  },
  {
    path: 'admin/trainers',
    component: TrainersList,
    canActivate: [roleGuard],
    data: { role: UserRole.Admin }
  },
  {
    path: 'admin/categories',
    component: CategoriesList,
    canActivate: [roleGuard],
    data: { role: UserRole.Admin }
  },
  {
    path: 'trainer',
    component: TrainerDashboard,
    canActivate: [roleGuard],
    data: { role: UserRole.Trainer }
  },
  {
    path: 'trainer/enrollments',
    component: TrainerEnrollments,
    canActivate: [roleGuard],
    data: { role: UserRole.Trainer }
  },
   {
    path: 'trainer/plans',
    component: MyPlans,
    canActivate: [roleGuard],
    data: { role: UserRole.Trainer }
    
  },
  {
    path: 'trainer/plans/:planId/sessions',
    component: PlanSessions,
    canActivate: [roleGuard],
    data: { role: UserRole.Trainer }
  },
    {
    path: 'trainer/plans/:planId/sessions/:sessionId/attendance',
    component: SessionAttendance,
    canActivate: [roleGuard],
    data: { role: UserRole.Trainer }
  },

  {
    path: 'trainer/profile',
    component: TrainerProfile,
    canActivate: [roleGuard],
    data: { role: UserRole.Trainer }
  },
  {
    path: 'trainer/plans/new',
    component: PlanForm,
    canActivate: [roleGuard],
    data: { role: UserRole.Trainer }
  },
  {
    path: 'trainer/plans/:id/edit',
    component: PlanForm,
    canActivate: [roleGuard],
    data: { role: UserRole.Trainer }
  },
  {
    path: 'client',
    component: ClientDashboard,
    canActivate: [roleGuard],
    data: { role: UserRole.Client }
  },
  {
    path: 'client/trainers',
    component: TrainerSearch,
    canActivate: [roleGuard],
    data: { role: UserRole.Client }
  },
  {
    path: 'client/trainers/:id',
    component: TrainerDetail,
    canActivate: [roleGuard],
    data: { role: UserRole.Client }
  },
  {
    path: 'client/enrollments',
    component: MyEnrollments,
    canActivate: [roleGuard],
    data: { role: UserRole.Client }
  },

  {
    path: 'client/schedule',
    component: MySchedule,
    canActivate: [roleGuard],
    data: { role: UserRole.Client }
  },

  {
      path: 'notifications',
      component: Notifications,
      canActivate: [authGuard]
    },
  { path: '', component: Home, canActivate: [authGuard], pathMatch: 'full' },
  { path: '**', redirectTo: '/login' }   // ← MORA biti poslednja!
];