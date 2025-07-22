import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { GrantService, Grant, CreateGrant } from '../../services/grant.service';
import { CategoryService, Category, PagedResult } from '../../services/category.service';

@Component({
  selector: 'app-grants',
  templateUrl: './grants.component.html',
  styleUrls: ['./grants.component.css']
})
export class GrantsComponent implements OnInit {
  grants: Grant[] = [];
  pagedResult: PagedResult<Grant> | null = null;
  categories: Category[] = [];
  Math = Math;
  
  // Filters
  selectedCategoryId: string = '';
  countryFilter: string = '';
  activeOnly: boolean = true;
  currentPage: number = 1;
  pageSize: number = 10;
  
  loading: boolean = false;
  error: string | null = null;
  
  showCreateForm: boolean = false;
  createForm: FormGroup;
  submitting: boolean = false;

  constructor(
    private grantService: GrantService,
    private categoryService: CategoryService,
    private fb: FormBuilder
  ) {
    this.createForm = this.fb.group({
      title: ['', [Validators.required, Validators.maxLength(200)]],
      description: ['', [Validators.required, Validators.maxLength(5000)]],
      country: ['', [Validators.required, Validators.maxLength(100)]],
      deadline: ['', [Validators.required]],
      requirements: ['', [Validators.maxLength(2000)]],
      fundingAmount: ['', [Validators.maxLength(100)]],
      categoryIds: [[], [Validators.required]]
    });
  }

  ngOnInit(): void {
    this.loadCategories();
    this.loadGrants();
  }

  loadCategories(): void {
    this.categoryService.getCategories('', 1, 100) // Get all categories for dropdown
      .subscribe({
        next: (result) => {
          this.categories = result.items;
        },
        error: (error) => {
          console.error('Error loading categories:', error);
        }
      });
  }

  loadGrants(): void {
    this.loading = true;
    this.error = null;
    
    const categoryId = this.selectedCategoryId || undefined;
    const country = this.countryFilter || undefined;
    
    this.grantService.getGrants(categoryId, country, this.activeOnly, this.currentPage, this.pageSize)
      .subscribe({
        next: (result) => {
          this.pagedResult = result;
          this.grants = result.items;
          this.loading = false;
        },
        error: (error) => {
          this.error = 'Failed to load grants';
          this.loading = false;
          console.error('Error loading grants:', error);
        }
      });
  }

  onFilterChange(): void {
    this.currentPage = 1;
    this.loadGrants();
  }

  onPageChange(page: number): void {
    this.currentPage = page;
    this.loadGrants();
  }

  toggleCreateForm(): void {
    this.showCreateForm = !this.showCreateForm;
    if (!this.showCreateForm) {
      this.createForm.reset();
      this.createForm.patchValue({ categoryIds: [], activeOnly: true });
    }
  }

  onCategorySelectionChange(categoryId: string, event: any): void {
    const isChecked = (event.target as HTMLInputElement).checked;
    const currentIds = this.createForm.get('categoryIds')?.value || [];
    let updatedIds;
    
    if (isChecked) {
      updatedIds = [...currentIds, categoryId];
    } else {
      updatedIds = currentIds.filter((id: string) => id !== categoryId);
    }
    
    this.createForm.patchValue({ categoryIds: updatedIds });
  }

  isCategorySelected(categoryId: string): boolean {
    const selectedIds = this.createForm.get('categoryIds')?.value || [];
    return selectedIds.includes(categoryId);
  }

  onSubmitCreate(): void {
    if (this.createForm.valid) {
      this.submitting = true;
      const formValue = this.createForm.value;
      
      // Convert deadline to ISO string format
      const deadline = new Date(formValue.deadline).toISOString();
      
      const createGrant: CreateGrant = {
        ...formValue,
        deadline: deadline
      };
      
      this.grantService.createGrant(createGrant)
        .subscribe({
          next: (grant) => {
            this.submitting = false;
            this.showCreateForm = false;
            this.createForm.reset();
            this.createForm.patchValue({ categoryIds: [] });
            this.loadGrants(); // Reload to get updated list
          },
          error: (error) => {
            this.submitting = false;
            this.error = 'Failed to create grant';
            console.error('Error creating grant:', error);
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

  formatDate(dateString: string): string {
    return new Date(dateString).toLocaleDateString();
  }

  isDeadlinePassed(dateString: string): boolean {
    return new Date(dateString) < new Date();
  }
}