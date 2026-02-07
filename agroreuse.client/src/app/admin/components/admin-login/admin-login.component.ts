import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { AdminAuthService } from '../../services/admin-auth.service';

@Component({
  selector: 'app-admin-login',
  templateUrl: './admin-login.component.html',
  styleUrls: ['./admin-login.component.css']
})
export class AdminLoginComponent implements OnInit {
  loginForm!: FormGroup;
  loading = false;
  errorMessage = '';
  showPassword = false;
  returnUrl = '/admin/dashboard';

  constructor(
    private formBuilder: FormBuilder,
    private authService: AdminAuthService,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    // Redirect if already logged in
    if (this.authService.isAuthenticated()) {
      this.router.navigate(['/admin/dashboard']);
      return;
    }

    // Get return url from route parameters or default to dashboard
    this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/admin/dashboard';

    // Initialize form
    this.loginForm = this.formBuilder.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      rememberMe: [false]
    });
  }

  get f() {
    return this.loginForm.controls;
  }

  togglePassword(): void {
    this.showPassword = !this.showPassword;
  }

  hideError(): void {
    this.errorMessage = '';
  }

  onSubmit(): void {
    // Clear previous error
    this.hideError();

    // Validate form
    if (this.loginForm.invalid) {
      Object.keys(this.loginForm.controls).forEach(key => {
        this.loginForm.get(key)?.markAsTouched();
      });
      return;
    }

    this.loading = true;
    const { email, password, rememberMe } = this.loginForm.value;

    this.authService.login(email, password, rememberMe).subscribe({
      next: () => {
        // Success - redirect to return URL
        this.router.navigate([this.returnUrl]);
      },
      error: (error) => {
        this.loading = false;
        
        // Handle different error types
        if (error.status === 401) {
          this.errorMessage = 'Invalid email or password';
        } else if (error.status === 400) {
          this.errorMessage = error.error?.message || 'Invalid request. Please check your credentials.';
        } else if (error.error instanceof ErrorEvent) {
          // Client-side error
          this.errorMessage = 'Network error. Please check your connection and try again.';
        } else if (error.message) {
          // Custom error message (like admin check)
          this.errorMessage = error.message;
        } else {
          this.errorMessage = 'An error occurred. Please try again.';
        }

        // Auto-hide error after 5 seconds
        setTimeout(() => this.hideError(), 5000);
      }
    });
  }
}
