import { Component, OnInit } from '@angular/core';
import { CategoryService } from '../../services/category.service';
import { Category } from '../../models/category.models';
import { environment } from '../../../../environments/environment';

@Component({
  selector: 'app-categories',
  templateUrl: './categories.component.html',
  styleUrls: ['./categories.component.css']
})
export class CategoriesComponent implements OnInit {
  categories: Category[] = [];
  loading = false;
  error = '';

  // Add/Edit state
  showAddForm = false;
  newCategoryName = '';
  newCategoryImage: File | null = null;
  newImagePreview: string | null = null;

  editingId: string | null = null;
  editName = '';
  editImage: File | null = null;
  editImagePreview: string | null = null;

  saving = false;

  constructor(private categoryService: CategoryService) {}

  ngOnInit(): void {
    this.loadCategories();
  }

  loadCategories(): void {
    this.loading = true;
    this.error = '';
    this.categoryService.getCategories().subscribe({
      next: (data) => {
        this.categories = data;
        this.loading = false;
      },
      error: () => {
        this.error = 'فشل تحميل الفئات';
        this.loading = false;
      }
    });
  }

  getImageUrl(imagePath?: string): string {
    if (!imagePath) return '';
    if (imagePath.startsWith('http')) return imagePath;
    return `${environment.apiUrl}/${imagePath}`;
  }

  // Add Category
  onNewImageSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files[0]) {
      this.newCategoryImage = input.files[0];
      const reader = new FileReader();
      reader.onload = (e) => {
        this.newImagePreview = e.target?.result as string;
      };
      reader.readAsDataURL(this.newCategoryImage);
    }
  }

  addCategory(): void {
    if (!this.newCategoryName.trim()) return;
    this.saving = true;
    this.categoryService.createCategory(this.newCategoryName.trim(), this.newCategoryImage || undefined).subscribe({
      next: (category) => {
        this.categories.push(category);
        this.resetAddForm();
        this.saving = false;
      },
      error: () => {
        alert('فشل إضافة الفئة');
        this.saving = false;
      }
    });
  }

  resetAddForm(): void {
    this.showAddForm = false;
    this.newCategoryName = '';
    this.newCategoryImage = null;
    this.newImagePreview = null;
  }

  // Edit Category
  startEdit(cat: Category): void {
    this.editingId = cat.id;
    this.editName = cat.name;
    this.editImage = null;
    this.editImagePreview = null;
  }

  onEditImageSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files[0]) {
      this.editImage = input.files[0];
      const reader = new FileReader();
      reader.onload = (e) => {
        this.editImagePreview = e.target?.result as string;
      };
      reader.readAsDataURL(this.editImage);
    }
  }

  saveEdit(cat: Category): void {
    if (!this.editName.trim()) return;
    this.saving = true;
    this.categoryService.updateCategory(cat.id, this.editName.trim(), this.editImage || undefined).subscribe({
      next: (res) => {
        cat.name = this.editName.trim();
        if (res.Data?.imagePath) {
          cat.imagePath = res.Data.imagePath;
        }
        this.cancelEdit();
        this.saving = false;
        this.loadCategories(); // Reload to get updated image
      },
      error: () => {
        alert('فشل تعديل الفئة');
        this.saving = false;
      }
    });
  }

  cancelEdit(): void {
    this.editingId = null;
    this.editName = '';
    this.editImage = null;
    this.editImagePreview = null;
  }

  // Delete Category
  deleteCategory(cat: Category): void {
    if (!confirm(`هل تريد حذف فئة "${cat.name}"؟`)) return;
    this.categoryService.deleteCategory(cat.id).subscribe({
      next: () => {
        this.categories = this.categories.filter(c => c.id !== cat.id);
      },
      error: () => alert('فشل حذف الفئة')
    });
  }
}
