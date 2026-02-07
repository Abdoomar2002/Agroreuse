# Admin Dashboard - Integration Architecture

## System Overview

```
┌─────────────────────────────────────────────────────────────┐
│                    Angular Admin Client                      │
│  (agroreuse.client)                                         │
│                                                             │
│  ┌────────────────────────────────────────────────────────┐│
│  │ Routes                                                 ││
│  │ ├─ /admin/login       → AdminLoginComponent           ││
│  │ ├─ /admin/dashboard   → AdminDashboardComponent       ││
│  │ └─ /                  → Redirect to /admin/login      ││
│  └────────────────────────────────────────────────────────┘│
│                                                             │
│  ┌────────────────────────────────────────────────────────┐│
│  │ Services                                               ││
│  │ ├─ AdminAuthService   → Handle login/logout, tokens   ││
│  │ └─ UsersService       → Fetch users from backend      ││
│  └────────────────────────────────────────────────────────┘│
│                                                             │
│  ┌────────────────────────────────────────────────────────┐│
│  │ Components                                             ││
│  │ ├─ AdminLoginComponent      → Login form UI           ││
│  │ ├─ AdminDashboardComponent  → Main dashboard layout   ││
│  │ └─ UsersManagementComponent → Users grid/filtering    ││
│  └────────────────────────────────────────────────────────┘│
│                                                             │
│  ┌────────────────────────────────────────────────────────┐│
│  │ Interceptors & Guards                                  ││
│  │ ├─ AuthInterceptor        → Auto-inject JWT token    ││
│  │ └─ AdminAuthGuard         → Protect admin routes     ││
│  └────────────────────────────────────────────────────────┘│
│                                                             │
└─────────────────────────────────────────────────────────────┘
         ↓ HTTP Requests (REST API)
┌─────────────────────────────────────────────────────────────┐
│              .NET 10 Server                                  │
│  (Agroreuse.Server)                                         │
│                                                             │
│  ┌────────────────────────────────────────────────────────┐│
│  │ Controllers                                            ││
│  │ ├─ AuthController                                     ││
│  │ │  ├─ POST /api/auth/login     → Authenticate user   ││
│  │ │  ├─ GET  /api/auth/me        → Current user info   ││
│  │ │  └─ Returns: JWT Token + User Info                 ││
│  │ │                                                     ││
│  │ └─ UsersController                                    ││
│  │    ├─ GET /api/users          → Get all users        ││
│  │    └─ [Authorize(Policy = "AdminOnly")]              ││
│  └────────────────────────────────────────────────────────┘│
│                                                             │
│  ┌────────────────────────────────────────────────────────┐│
│  │ Authentication & Authorization                        ││
│  │ ├─ UserManager<ApplicationUser>                       ││
│  │ ├─ IJwtTokenService → Generate JWT tokens            ││
│  │ ├─ Policy: "AdminOnly" → Only Admin users allowed    ││
│  │ └─ Claim: UserType → Admin/Farmer/Factory            ││
│  └────────────────────────────────────────────────────────┘│
│                                                             │
│  ┌────────────────────────────────────────────────────────┐│
│  │ Database (Entity Framework Core)                      ││
│  │ ├─ ApplicationUser                                    ││
│  │ │  ├─ Id                                              ││
│  │ │  ├─ Email, FullName, PhoneNumber                   ││
│  │ │  ├─ Type (UserType: Admin/Farmer/Factory)          ││
│  │ │  ├─ IsLocked, CreatedAt                            ││
│  │ │  └─ ImagePath                                      ││
│  │ │                                                     ││
│  │ └─ Other Entities (ContactUs, etc.)                  ││
│  └────────────────────────────────────────────────────────┘│
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

## Data Flow Diagrams

### 1. Login Flow

```
┌──────────────────────────────────────────────────────────────┐
│ 1. User enters email & password in AdminLoginComponent       │
└──────────────────────┬───────────────────────────────────────┘
                       ↓
┌──────────────────────────────────────────────────────────────┐
│ 2. AdminAuthService.login() called                           │
│    POST /api/auth/login                                      │
│    Body: { email, password, userType: "Admin" }             │
└──────────────────────┬───────────────────────────────────────┘
                       ↓
┌──────────────────────────────────────────────────────────────┐
│ 3. Backend AuthController validates credentials             │
│    ├─ Find user by email                                     │
│    ├─ Verify password with UserManager                       │
│    ├─ Check user type is Admin                               │
│    └─ Generate JWT token                                     │
└──────────────────────┬───────────────────────────────────────┘
                       ↓
┌──────────────────────────────────────────────────────────────┐
│ 4. Response: { token, email, fullName, id }                │
└──────────────────────┬───────────────────────────────────────┘
                       ↓
┌──────────────────────────────────────────────────────────────┐
│ 5. Service stores token & user data                          │
│    ├─ localStorage (if "Remember me" checked)                │
│    └─ sessionStorage (default)                               │
└──────────────────────┬───────────────────────────────────────┘
                       ↓
┌──────────────────────────────────────────────────────────────┐
│ 6. Navigate to /admin/dashboard                              │
└──────────────────────────────────────────────────────────────┘
```

### 2. User Listing Flow

```
┌──────────────────────────────────────────────────────────────┐
│ 1. User clicks "المستخدمون" (Users) in AdminDashboardComp    │
│    currentView = 'users'                                     │
└──────────────────────┬───────────────────────────────────────┘
                       ↓
┌──────────────────────────────────────────────────────────────┐
│ 2. UsersManagementComponent.ngOnInit()                        │
│    loadUsers() called                                        │
└──────────────────────┬───────────────────────────────────────┘
                       ↓
┌──────────────────────────────────────────────────────────────┐
│ 3. UsersService.getAllUsers()                                │
│    GET /api/users                                            │
│    Headers: { Authorization: Bearer {token} }                │
│    (Added by AuthInterceptor)                                │
└──────────────────────┬───────────────────────────────────────┘
                       ↓
┌──────────────────────────────────────────────────────────────┐
│ 4. Backend UsersController.GetAllUsers()                     │
│    ├─ [Authorize] - Verify JWT token                         │
│    ├─ [Authorize(Policy = "AdminOnly")] - Check Admin claim  │
│    ├─ Fetch all users from database                          │
│    └─ Return UserDto[] list                                  │
└──────────────────────┬───────────────────────────────────────┘
                       ↓
┌──────────────────────────────────────────────────────────────┐
│ 5. Response: Array of User objects                           │
│    [                                                          │
│      {                                                        │
│        id, email, fullName, phoneNumber, address,            │
│        type: "Farmer"/"Factory"/"Admin",                     │
│        createdAt, isLocked, imagePath                        │
│      },                                                       │
│      ...                                                      │
│    ]                                                         │
└──────────────────────┬───────────────────────────────────────┘
                       ↓
┌──────────────────────────────────────────────────────────────┐
│ 6. Component processes data                                  │
│    ├─ allUsers = response                                    │
│    ├─ farmers = filter(type === 'Farmer')                    │
│    └─ factories = filter(type === 'Factory')                 │
└──────────────────────┬───────────────────────────────────────┘
                       ↓
┌──────────────────────────────────────────────────────────────┐
│ 7. UsersManagementComponent renders grid with users          │
│    Grouped by tab: All | Farmers | Factories                │
└──────────────────────────────────────────────────────────────┘
```

### 3. Protected Route Flow

```
┌──────────────────────────────────────────────────────────────┐
│ User navigates to /admin/dashboard                           │
└──────────────────────┬───────────────────────────────────────┘
                       ↓
┌──────────────────────────────────────────────────────────────┐
│ AdminAuthGuard.canActivate() called                          │
│ Check: isAuthenticated() ?                                   │
└──────────────────────┬───────────────────────────────────────┘
                       ↓
                  ✓ YES / ✗ NO
                    /  \
                   /    \
                  ↓      ↓
         ┌─────────┐   ┌─────────────────────────┐
         │ Allow   │   │ Redirect to /admin/login│
         │ access  │   │ with returnUrl param    │
         └─────────┘   └─────────────────────────┘
             ↓
    ┌──────────────────────────────────────┐
    │ AdminAuthGuard.isAuthenticated():     │
    │ ├─ token = getToken() ?               │
    │ ├─ user = currentUserValue ?          │
    │ └─ user.userType === 'Admin' ?       │
    └──────────────────────────────────────┘
```

## Component Interaction Diagram

```
┌─────────────────────────────────────────────────────────┐
│                   AdminDashboardComponent               │
│  - currentUser: AdminUser                               │
│  - currentView: 'dashboard' | 'users'                   │
│  + navigateTo()                                         │
│  + logout()                                             │
└──────────┬──────────────────────────────────────────────┘
           │
           ├─ [Dashboard View]
           │  ├─ Static welcome message
           │  └─ Statistics cards (placeholder data)
           │
           └─ [Users View]
              └─ <app-users-management>
                 │
                 └─ UsersManagementComponent
                    - allUsers: User[]
                    - farmers: User[]
                    - factories: User[]
                    - selectedTab: 'all'|'farmers'|'factories'
                    + loadUsers()
                    + selectTab()
                    - displayedUsers: computed property
                    │
                    └─ Uses: UsersService
                       └─ getAllUsers(): Observable<User[]>
```

## Service Dependencies

```
AdminDashboardComponent
    ├─ AdminAuthService
    │  ├─ HttpClient
    │  └─ BehaviorSubject<AdminUser>
    │
    └─ Router

AdminLoginComponent
    ├─ AdminAuthService
    ├─ Router
    ├─ ActivatedRoute
    └─ FormBuilder (ReactiveFormsModule)

UsersManagementComponent
    └─ UsersService
       └─ HttpClient

AuthInterceptor
    └─ AdminAuthService
       └─ getToken()

AdminAuthGuard
    ├─ AdminAuthService
    │  └─ isAuthenticated()
    └─ Router
```

## Authentication Token Flow

```
1. LOGIN
   User Email/Password
        ↓
   Backend validates
        ↓
   Returns JWT Token
        ↓
   Store in localStorage/sessionStorage
        ↓

2. SUBSEQUENT REQUESTS
   AuthInterceptor reads token from storage
        ↓
   Adds to request header: Authorization: Bearer {token}
        ↓
   Backend verifies JWT signature
        ↓
   Backend checks claims (UserType: Admin)
        ↓
   If valid → Process request
   If invalid → Return 401 Unauthorized
        ↓

3. LOGOUT
   Remove token from storage
        ↓
   Clear user data
        ↓
   Redirect to login
```

## File Structure

```
agroreuse.client/
├── src/
│   ├── app/
│   │   ├── admin/                          # Feature module
│   │   │   ├── components/
│   │   │   │   ├── admin-login/            # Login page
│   │   │   │   ├── admin-dashboard/        # Main dashboard
│   │   │   │   └── users-management/       # Users management
│   │   │   ├── services/
│   │   │   │   ├── admin-auth.service.ts   # Auth logic
│   │   │   │   └── users.service.ts        # Users data
│   │   │   ├── guards/
│   │   │   │   └── admin-auth.guard.ts     # Route protection
│   │   │   ├── interceptors/
│   │   │   │   └── auth.interceptor.ts     # JWT injection
│   │   │   ├── models/
│   │   │   │   ├── auth.models.ts          # Auth types
│   │   │   │   └── user.models.ts          # User types
│   │   │   └── admin.module.ts             # Feature module
│   │   │
│   │   ├── app-routing.module.ts           # Main routes
│   │   ├── app.module.ts                   # Root module
│   │   ├── app.component.ts                # Root component
│   │   └── app.component.html              # Root template
│   │
│   └── main.ts                             # Entry point
│
├── ADMIN_DASHBOARD_README.md               # Documentation
├── TESTING_GUIDE.md                        # Testing guide
└── angular.json                            # Angular config
```

---

This architecture ensures:
✅ Clean separation of concerns
✅ Reusable services
✅ Protected routes
✅ Automatic token injection
✅ Type safety with TypeScript
✅ Responsive UI
✅ Arabic language support
✅ Backend API integration
