import { Routes } from '@angular/router';
import { Login } from './login/login';
import { Upload } from './upload/upload';

export const routes: Routes = [
  { path: 'login', component: Login },
  { path: 'upload', component: Upload },
  { path: '', redirectTo: '/login', pathMatch: 'full' },
  { path: '**', redirectTo: '/login' }
];
