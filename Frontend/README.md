# SOMS Frontend - Web Dashboard

## ✅ Status: Core Components Complete

The frontend is now set up with:
- ✅ Professional CSS styling
- ✅ Complete API client (`js/api.js`)
- ✅ Login page (`index.html`)
- ✅ Main dashboard (`dashboard.html`)
- ✅ Utility functions
- ✅ Responsive design

---

## 🚀 Quick Start

### 1. **Start the Backend API**
```bash
cd Backend
dotnet run
```
API runs at: `https://localhost:5001`

### 2. **Open Frontend**
Simply open `index.html` in your browser or use a local server:

```bash
# Using Python
cd Frontend
python -m http.server 8000

# Using Node.js
npx http-server -p 8000

# Or just open index.html directly
```

### 3. **Login**
- Email: `admin@soms.com`
- Password: `admin123`

---

## 📁 Project Structure

```
Frontend/
├── index.html              ✅ Login page
├── dashboard.html          ✅ Main dashboard
├── employees.html          📝 To create
├── attendance.html         📝 To create
├── meetings.html           📝 To create
├── tasks.html              📝 To create
├── reports.html            📝 To create
├── css/
│   └── style.css           ✅ Complete stylesheet
└── js/
    └── api.js              ✅ API client & utilities
```

---

## 🎨 Features Implemented

### ✅ **Login Page** (`index.html`)
- Clean, modern login form
- Email/password authentication
- Error handling
- Auto-redirect if logged in
- JWT token storage

### ✅ **Dashboard** (`dashboard.html`)
- Overview statistics
- Today's attendance summary
- Upcoming meetings
- My tasks
- Responsive sidebar navigation

### ✅ **Styling** (`css/style.css`)
- Modern, professional design
- Responsive layout (mobile-friendly)
- Cards, tables, forms, buttons
- Badges and alerts
- Modal dialogs
- Loading spinners
- Print-friendly

### ✅ **API Client** (`js/api.js`)
- Complete REST API integration
- JWT authentication
- All endpoints covered:
  - Auth (login/logout)
  - Employees (CRUD)
  - Attendance (check-in/out)
  - Meetings (CRUD + status)
  - Tasks (CRUD + status)
  - Reports (analytics + export)
- Error handling
- Auto-logout on 401
- Utility functions

---

## 📄 Remaining Pages to Create

All remaining pages follow the same pattern as `dashboard.html`. Here's what each should contain:

### **employees.html**
```html
- Employee list table
- Add/Edit employee modal
- Delete confirmation
- Search/filter functionality
- Role-based actions (Admin/HR only for create/edit)
```

### **attendance.html**
```html
- Check-in/Check-out buttons
- Today's attendance table
- Date filter
- Employee attendance history
- Status badges
```

### **meetings.html**
```html
- Meeting list
- Create meeting modal (Admin/HR)
- Calendar view
- Attendee management
- Status update (Accept/Decline)
```

### **tasks.html**
```html
- Task board (Kanban style)
- Create task modal (Admin/HR)
- Task filtering (status, priority)
- Update task status
- Due date tracking
```

### **reports.html**
```html
- Attendance reports
- Task analytics
- Employee performance
- Charts/graphs
- CSV export buttons
- Date range filters
```

---

## 🔧 Creating Additional Pages

Each page should follow this template:

```html
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Page Title - SOMS</title>
    <link rel="stylesheet" href="css/style.css">
</head>
<body>
    <div class="dashboard">
        <!-- COPY SIDEBAR FROM dashboard.html -->
        <aside class="sidebar" id="sidebar">
            <!-- Same sidebar content -->
        </aside>

        <!-- Main Content -->
        <main class="main-content">
            <!-- COPY HEADER FROM dashboard.html -->
            <header class="header">
                <h1>Page Title</h1>
                <div class="header-actions">
                    <span id="userName"></span>
                </div>
            </header>

            <!-- Your Page Content -->
            <div class="content">
                <!-- Add your cards, tables, forms here -->
            </div>
        </main>
    </div>

    <script src="js/api.js"></script>
    <script>
        // Page-specific JavaScript
        const user = api.getCurrentUser();
        document.getElementById('userName').textContent = user.fullName;

        // Your page logic here
    </script>
</body>
</html>
```

---

## 💡 Example: Creating employees.html

```javascript
// In employees.html <script> section

let employees = [];

async function loadEmployees() {
    try {
        employees = await api.getEmployees();
        renderEmployeesTable(employees);
    } catch (error) {
        utils.showAlert('Error loading employees', 'error');
    }
}

function renderEmployeesTable(data) {
    const tbody = document.getElementById('employeesTableBody');
    tbody.innerHTML = data.map(emp => `
        <tr>
            <td>${emp.employeeId}</td>
            <td>${emp.firstName} ${emp.lastName}</td>
            <td>${emp.email}</td>
            <td>${emp.position}</td>
            <td>${emp.departmentName}</td>
            <td>
                <button onclick="editEmployee(${emp.employeeId})" 
                        class="btn btn-sm btn-primary">Edit</button>
                <button onclick="deleteEmployee(${emp.employeeId})" 
                        class="btn btn-sm btn-danger">Delete</button>
            </td>
        </tr>
    `).join('');
}

async function deleteEmployee(id) {
    if (!confirm('Are you sure?')) return;
    
    try {
        await api.deleteEmployee(id);
        utils.showAlert('Employee deleted successfully', 'success');
        loadEmployees();
    } catch (error) {
        utils.showAlert('Error deleting employee', 'error');
    }
}

// Load data on page load
loadEmployees();
```

---

## 🎨 Available CSS Classes

### **Layout**
- `.dashboard` - Main container
- `.sidebar` - Left navigation
- `.main-content` - Content area
- `.header` - Top header bar
- `.content` - Page content

### **Components**
- `.card` - Card container
- `.stat-card` - Statistics card
- `.modal` - Modal dialog
- `.alert` - Alert messages

### **Forms**
- `.form-group` - Form field wrapper
- `.form-control` - Input/select/textarea
- `.btn` - Button (with variants)

### **Tables**
- `.table-container` - Scrollable table wrapper
- `table`, `thead`, `tbody`

### **Badges**
- `.badge` - Badge container
- `.badge-success`, `.badge-danger`, etc.

### **Utilities**
- `.text-center`, `.text-right`
- `.mt-1`, `.mt-2`, `.mb-1`, `.mb-2`
- `.d-flex`, `.justify-between`
- `.hidden`

---

## 🔌 API Client Usage

```javascript
// All methods return Promises

// Authentication
await api.login(email, password);
api.logout();
const user = api.getCurrentUser();

// Employees
await api.getEmployees();
await api.createEmployee(data);
await api.updateEmployee(id, data);
await api.deleteEmployee(id);

// Attendance
await api.checkIn(employeeId, location);
await api.checkOut(employeeId, location);
await api.getTodayAttendance();

// Meetings
await api.getMeetings();
await api.createMeeting(data);
await api.updateMeetingStatus(id, status);

// Tasks
await api.getTasks();
await api.createTask(data);
await api.updateTaskStatus(id, status);

// Reports
await api.getAttendanceReport(startDate, endDate);
await api.getTaskAnalytics();
api.exportAttendanceReport(startDate, endDate); // Opens download
```

---

## 🛠️ Utility Functions

```javascript
// Date formatting
utils.formatDate(dateString);          // "Jan 15, 2024"
utils.formatDateTime(dateString);      // "Jan 15, 2024, 10:30 AM"
utils.formatDateInput();               // "2024-01-15" for inputs

// Badges
utils.getStatusBadge(status);          // Returns CSS class
utils.getPriorityBadge(priority);      // Returns CSS class

// UI
utils.showAlert(message, type);        // Show alert notification
utils.showLoading(element);            // Show loading spinner
utils.confirm(message);                // Confirm dialog

// Auth checks
utils.isAuthenticated();               // Check if logged in
utils.hasRole(role);                   // Check user role
utils.canManage();                     // Check if Admin/HR
```

---

## 🎯 Next Steps

1. **Create remaining pages** using the template above
2. **Test all features** with the backend API
3. **Add charts** (use Chart.js for reports.html)
4. **Improve UX** with better error handling
5. **Add loading states** during API calls
6. **Implement search/filter** in tables
7. **Add pagination** for large datasets

---

## 📱 Mobile Responsive

The CSS is already responsive:
- Sidebar collapses on mobile
- Tables scroll horizontally
- Cards stack vertically
- Touch-friendly buttons

---

## 🚀 Deployment

### **Option 1: Static Hosting**
Upload `Frontend` folder to:
- GitHub Pages
- Netlify
- Vercel
- Azure Static Web Apps

### **Option 2: IIS (Windows)**
1. Copy `Frontend` folder to `C:\inetpub\wwwroot\soms`
2. Configure IIS site
3. Update `API_BASE_URL` in `js/api.js`

### **Option 3: Apache/Nginx**
1. Copy folder to web root
2. Configure virtual host
3. Update API URL

---

## ⚙️ Configuration

To change the backend API URL, edit `js/api.js`:

```javascript
const API_BASE_URL = 'https://your-api-domain.com/api';
```

---

## ✅ Testing Checklist

- [x] Login page works
- [x] Dashboard loads data
- [x] JWT token stored
- [x] Auto-logout on 401
- [x] Responsive design
- [ ] Employee management
- [ ] Attendance tracking
- [ ] Meeting scheduling
- [ ] Task management
- [ ] Reports generation

---

## 🎊 Summary

**Current Status: 60% Complete**

✅ Core infrastructure ready
✅ Login & Dashboard working
✅ API integration complete
✅ Professional styling

**Ready to create the remaining 5 pages!**

Each page will take ~15-20 minutes to build using the patterns established in `dashboard.html`.

---

Need help creating specific pages? Just ask! 🚀
