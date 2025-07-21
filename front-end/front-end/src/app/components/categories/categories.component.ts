import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { CategoryService, Category, CreateCategory, PagedResult } from '../../services/category.service';

@Component({
  selector: 'app-categories',
  templateUrl: './categories.component.html',
  styleUrls: ['./categories.component.css']
})
export class CategoriesComponent implements OnInit {
  categories: Category[] = [];
  pagedResult: PagedResult<Category> | null = null;
  Math = Math;
  searchTerm: string = '';
  currentPage: number = 1;
  pageSize: number = 10;
  loading: boolean = false;
  error: string | null = null;
  
  showCreateForm: boolean = false;
  createForm: FormGroup;
  submitting: boolean = false;

  constructor(
    private categoryService: CategoryService,
    private fb: FormBuilder
  ) {
    this.createForm = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(100)]],
      description: ['', [Validators.maxLength(500)]]
    });
  }

  ngOnInit(): void {
    this.loadCategories();
  }

  loadCategories(): void {
    this.loading = true;
    this.error = null;
    
    this.categoryService.getCategories(this.searchTerm, this.currentPage, this.pageSize)
      .subscribe({
        next: (result) => {
          this.pagedResult = result;
          this.categories = result.items;
          this.loading = false;
        },
        error: (error) => {
          this.error = 'Failed to load categories';
          this.loading = false;
          console.error('Error loading categories:', error);
        }
      });
  }

  onSearchChange(): void {
    this.currentPage = 1;
    this.loadCategories();
  }

  onPageChange(page: number): void {
    this.currentPage = page;
    this.loadCategories();
  }

  toggleCreateForm(): void {
    this.showCreateForm = !this.showCreateForm;
    if (!this.showCreateForm) {
      this.createForm.reset();
    }
  }

  onSubmitCreate(): void {
    if (this.createForm.valid) {
      this.submitting = true;
      const createCategory: CreateCategory = this.createForm.value;
      
      this.categoryService.createCategory(createCategory)
        .subscribe({
          next: (category) => {
            this.submitting = false;
            this.showCreateForm = false;
            this.createForm.reset();
            this.loadCategories(); // Reload to get updated list
          },
          error: (error) => {
            this.submitting = false;
            this.error = 'Failed to create category';
            console.error('Error creating category:', error);
          }
        });
    }
  }

  getPaginationPages(): number[] {
    if (!this.pagedResult) return [];
    
    const totalPages = this.pagedResult.totalPages;
    const currentPage = this.pagedResult.page;
    const pages: number[] = [];
    
    // Show up to 5 pages around current page
    const start = Math.max(1, currentPage - 2);
    const end = Math.min(totalPages, currentPage + 2);
    
    for (let i = start; i <= end; i++) {
      pages.push(i);
    }
    
    return pages;
  }
}