import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { Category, PagedResult } from './category.service';

export interface Grant {
  id: number;
  title: string;
  description: string;
  country: string;
  deadline: string;
  requirements: string;
  fundingAmount: string;
  createdAt: string;
  updatedAt?: string;
  isActive: boolean;
  categories: Category[];
}

export interface CreateGrant {
  title: string;
  description: string;
  country: string;
  deadline: string;
  requirements: string;
  fundingAmount: string;
  categoryIds: string[];
}

@Injectable({
  providedIn: 'root'
})
export class GrantService {
  private readonly apiUrl = `${environment.apiUrl}/grants`;

  constructor(private http: HttpClient) { }

  getGrants(
    categoryId?: string, 
    country?: string, 
    activeOnly: boolean = true, 
    page: number = 1, 
    pageSize: number = 10
  ): Observable<PagedResult<Grant>> {
    let params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString())
      .set('activeOnly', activeOnly.toString());

    if (categoryId) {
      params = params.set('categoryId', categoryId);
    }

    if (country) {
      params = params.set('country', country);
    }

    return this.http.get<PagedResult<Grant>>(this.apiUrl, { params });
  }

  getGrant(id: number): Observable<Grant> {
    return this.http.get<Grant>(`${this.apiUrl}/${id}`);
  }

  createGrant(grant: CreateGrant): Observable<Grant> {
    return this.http.post<Grant>(this.apiUrl, grant);
  }

  updateGrant(id: number, grant: CreateGrant): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, grant);
  }

  deleteGrant(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}