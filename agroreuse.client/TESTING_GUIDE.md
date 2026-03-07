# Admin Dashboard Testing Guide

## Prerequisites

- Both Angular client and .NET server are running
- Admin user exists in the database with:
  - Email: (any valid email)
  - Password: (configured password)
  - UserType: Admin
  - IsLocked: false

## Testing Steps

### Step 1: Login Page
1. Navigate to `http://localhost:[client-port]/admin/login`
2. You should see:
   - Dark green background (#1E4D2B)
   - White login panel in the center
   - Agroreuse logo
   - Email and password fields
   - "Remember me" checkbox
   - "Sign In" button

### Step 2: Test Login
1. Enter admin email
2. Enter admin password
3. Click "Sign In"
4. Expected: Redirect to dashboard
5. Check browser console for any errors

### Step 3: Dashboard View
1. You should see:
   - Right-side panel with green background (#1E4D2B)
   - Agroreuse logo and "Agroreuse" text in header
   - Two navigation buttons:
     - "لوحة التحكم" (Dashboard) - Active by default
     - "المستخدمون" (Users)
   - "تسجيل الخروج" (Logout) button at bottom
   - Main content area with welcome message
   - Statistics cards

### Step 4: Users Management
1. Click "المستخدمون" (Users) button in right panel
2. Users management page should load with:
   - Tab buttons for: All, Farmers, Factories
   - User statistics showing counts
   - Grid of user cards
3. Each user card shows:
   - Avatar (image or initials)
   - Full name
   - User type (in Arabic)
   - Email
   - Phone (if available)
   - Address (if available)
   - Registration date
   - Status (Active/Locked)

### Step 5: Test User Filtering
1. Click "المزارعون" (Farmers) tab - should show only farmers
2. Click "المصانع" (Factories) tab - should show only factories
3. Click "الكل" (All) tab - should show all users

### Step 6: Responsive Design
1. Open DevTools (F12)
2. Toggle device toolbar (Ctrl+Shift+M)
3. Test at different breakpoints:
   - Desktop (1024px+)
   - Tablet (768px - 1024px)
   - Mobile (480px - 768px)
   - Small Mobile (< 480px)
4. All elements should resize properly

### Step 7: Test Logout
1. Click "تسجيل الخروج" (Logout) button
2. Confirmation dialog should appear with text: "هل تريد تسجيل الخروج؟"
3. Click OK to confirm
4. Should redirect to login page
5. Session should be cleared (tokens removed)

### Step 8: Session Persistence
1. Login again
2. Check "Remember me" checkbox
3. Close and reopen the browser
4. You should still be logged in (token in localStorage)
5. Without "Remember me", login expires when browser closes

## Expected API Calls

### During Login
```
POST /api/auth/login
Headers: Content-Type: application/json
Body: {
  "email": "admin@example.com",
  "password": "password",
  "userType": "Admin"
}
Response: {
  "token": "eyJhbGc...",
  "email": "admin@example.com",
  "fullName": "Admin Name",
  "id": "user-id"
}
```

### Loading Users
```
GET /api/users
Headers: 
  - Authorization: Bearer {token}
  - Content-Type: application/json
Response: Array of User objects
[
  {
    "id": "user-id",
    "email": "farmer@example.com",
    "fullName": "Farmer Name",
    "phoneNumber": "+1234567890",
    "address": "Farm Location",
    "type": "Farmer",
    "createdAt": "2024-01-15T10:30:00Z",
    "isLocked": false,
    "imagePath": null
  },
  ...
]
```

## Common Issues & Solutions

### Issue 1: "Invalid email or password"
- ✅ Solution: Check if admin user exists in database with correct credentials
- ✅ Verify UserType is "Admin"
- ✅ Verify IsLocked is false

### Issue 2: "Access denied. Admin privileges required"
- ✅ Solution: Check that the user's Type/UserType is set to "Admin"
- ✅ Verify the login response includes correct user type

### Issue 3: Users page shows empty
- ✅ Solution: Check if /api/users endpoint returns data
- ✅ Verify the Authorization header is being sent correctly
- ✅ Check browser DevTools Network tab to see API response
- ✅ Make sure at least one user exists in the database

### Issue 4: Page not loading after login
- ✅ Solution: Check browser console for JavaScript errors
- ✅ Verify Angular app is running on correct port
- ✅ Check that admin routes are properly configured

### Issue 5: "localhost refused to connect"
- ✅ Solution: Ensure both Angular client and .NET server are running
- ✅ Check CORS settings on server (should allow frontend origin)
- ✅ Verify proxy settings in proxy.conf.js if using local development

## Browser DevTools Checklist

1. **Network Tab**
   - Check API calls are being made
   - Verify 200 status codes for successful requests
   - Check Authorization header is included

2. **Console Tab**
   - No JavaScript errors
   - No CORS errors
   - Check for warning messages

3. **Storage Tab**
   - localStorage should contain adminToken and adminUser (if "Remember me" checked)
   - sessionStorage should contain these if not using "Remember me"

4. **Application Tab**
   - Verify tokens are stored in correct storage
   - Check token format (should be JWT)

## Performance Testing

1. Load time: Dashboard should load in < 2 seconds
2. Users list: Loading 1000+ users should be handled gracefully with loading state
3. Responsive: No layout shifts when resizing
4. Memory: No significant memory leaks after extended use

## Accessibility Testing

- [ ] Keyboard navigation (Tab through fields)
- [ ] Screen reader compatibility (ARIA labels)
- [ ] Color contrast ratios meet WCAG standards
- [ ] Form labels properly associated with inputs
- [ ] Error messages are clear and accessible

## Security Testing

- [ ] JWT token included in all protected API calls
- [ ] Non-admin users cannot access `/admin/dashboard`
- [ ] Logout clears all stored credentials
- [ ] Sensitive data not exposed in localStorage
- [ ] Cross-site scripting (XSS) protection

---

**Test Date**: ___________
**Tester**: ___________
**Status**: ☐ PASS ☐ FAIL
**Notes**: ___________
