import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CategoriesComponent } from './components/categories/categories.component';
import { GrantsComponent } from './components/grants/grants.component';

const routes: Routes = [
  { path: '', redirectTo: '/grants', pathMatch: 'full' },
  { path: 'grants', component: GrantsComponent },
  { path: 'categories', component: CategoriesComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
