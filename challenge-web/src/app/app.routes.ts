import { Routes } from '@angular/router';
import { Upload } from './upload/upload';

export const routes: Routes = [
  { path: 'upload', component: Upload },
  { path: '', redirectTo: '/upload', pathMatch: 'full' },
  { path: '**', redirectTo: '/upload' }
];
