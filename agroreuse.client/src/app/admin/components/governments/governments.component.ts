import { Component, OnInit } from '@angular/core';
import { LocationService } from '../../services/location.service';
import { Government, City } from '../../models/location.models';

@Component({
  selector: 'app-governments',
  templateUrl: './governments.component.html',
  styleUrls: ['./governments.component.css']
})
export class GovernmentsComponent implements OnInit {
  governments: Government[] = [];
  loading = false;
  error = '';

  // Modal state
  showCityModal = false;
  selectedGovernment: Government | null = null;
  cityModalLoading = false;

  // Add/Edit forms
  showAddGovForm = false;
  newGovName = '';
  editingGovId: string | null = null;
  editGovName = '';

  showAddCityForm = false;
  newCityName = '';
  editingCityId: string | null = null;
  editCityName = '';

  constructor(private locationService: LocationService) {}

  ngOnInit(): void {
    this.loadGovernments();
  }

  loadGovernments(): void {
    this.loading = true;
    this.error = '';
    this.locationService.getGovernments(true).subscribe({
      next: (data) => {
        this.governments = data;
        this.loading = false;
      },
      error: () => {
        this.error = 'فشل تحميل المحافظات';
        this.loading = false;
      }
    });
  }

  // Government CRUD
  addGovernment(): void {
    if (!this.newGovName.trim()) return;
    this.locationService.createGovernment(this.newGovName.trim()).subscribe({
      next: (gov) => {
        this.governments.push({ ...gov, cities: [] });
        this.newGovName = '';
        this.showAddGovForm = false;
      },
      error: () => alert('فشل إضافة المحافظة')
    });
  }

  startEditGov(gov: Government): void {
    this.editingGovId = gov.id;
    this.editGovName = gov.name;
  }

  saveEditGov(gov: Government): void {
    if (!this.editGovName.trim()) return;
    this.locationService.updateGovernment(gov.id, this.editGovName.trim()).subscribe({
      next: () => {
        gov.name = this.editGovName.trim();
        this.editingGovId = null;
        this.editGovName = '';
      },
      error: () => alert('فشل تعديل المحافظة')
    });
  }

  cancelEditGov(): void {
    this.editingGovId = null;
    this.editGovName = '';
  }

  deleteGovernment(gov: Government): void {
    if (!confirm(`هل تريد حذف محافظة "${gov.name}"؟`)) return;
    this.locationService.deleteGovernment(gov.id).subscribe({
      next: () => {
        this.governments = this.governments.filter(g => g.id !== gov.id);
      },
      error: () => alert('فشل حذف المحافظة')
    });
  }

  // City Modal
  openCityModal(gov: Government): void {
    this.selectedGovernment = gov;
    this.showCityModal = true;
    this.showAddCityForm = false;
    this.editingCityId = null;
  }

  closeCityModal(): void {
    this.showCityModal = false;
    this.selectedGovernment = null;
    this.showAddCityForm = false;
    this.newCityName = '';
    this.editingCityId = null;
  }

  // City CRUD
  addCity(): void {
    if (!this.newCityName.trim() || !this.selectedGovernment) return;
    this.cityModalLoading = true;
    this.locationService.createCity(this.newCityName.trim(), this.selectedGovernment.id).subscribe({
      next: (city) => {
        if (this.selectedGovernment) {
          if (!this.selectedGovernment.cities) this.selectedGovernment.cities = [];
          this.selectedGovernment.cities.push(city);
        }
        this.newCityName = '';
        this.showAddCityForm = false;
        this.cityModalLoading = false;
      },
      error: () => {
        alert('فشل إضافة المدينة');
        this.cityModalLoading = false;
      }
    });
  }

  startEditCity(city: City): void {
    this.editingCityId = city.id;
    this.editCityName = city.name;
  }

  saveEditCity(city: City): void {
    if (!this.editCityName.trim()) return;
    this.cityModalLoading = true;
    this.locationService.updateCity(city.id, this.editCityName.trim()).subscribe({
      next: () => {
        city.name = this.editCityName.trim();
        this.editingCityId = null;
        this.editCityName = '';
        this.cityModalLoading = false;
      },
      error: () => {
        alert('فشل تعديل المدينة');
        this.cityModalLoading = false;
      }
    });
  }

  cancelEditCity(): void {
    this.editingCityId = null;
    this.editCityName = '';
  }

  deleteCity(city: City): void {
    if (!confirm(`هل تريد حذف مدينة "${city.name}"؟`)) return;
    this.cityModalLoading = true;
    this.locationService.deleteCity(city.id).subscribe({
      next: () => {
        if (this.selectedGovernment?.cities) {
          this.selectedGovernment.cities = this.selectedGovernment.cities.filter(c => c.id !== city.id);
        }
        this.cityModalLoading = false;
      },
      error: () => {
        alert('فشل حذف المدينة');
        this.cityModalLoading = false;
      }
    });
  }
}
