// SOMS API Client

const API_BASE_URL = 'http://localhost:5086/api';

// API Client Class
class APIClient {
    constructor() {
        this.baseURL = API_BASE_URL;
        this.token = localStorage.getItem('token');
    }

    // Get auth headers
    getHeaders(includeAuth = true) {
        const headers = {
            'Content-Type': 'application/json'
        };

        if (includeAuth && this.token) {
            headers['Authorization'] = `Bearer ${this.token}`;
        }

        return headers;
    }

    // Handle API response
    async handleResponse(response) {
        if (!response.ok) {
            if (response.status === 401) {
                // Token expired or invalid
                this.logout();
                window.location.href = 'index.html';
                throw new Error('Session expired. Please login again.');
            }

            const error = await response.json().catch(() => ({ message: 'An error occurred' }));
            throw new Error(error.message || `HTTP error! status: ${response.status}`);
        }

        // Handle empty responses
        const text = await response.text();
        return text ? JSON.parse(text) : null;
    }

    // Generic API request method
    async request(endpoint, options = {}) {
        try {
            const response = await fetch(`${this.baseURL}${endpoint}`, {
                ...options,
                headers: this.getHeaders(options.auth !== false)
            });

            return await this.handleResponse(response);
        } catch (error) {
            console.error('API Error:', error);
            throw error;
        }
    }

    // Authentication
    async login(email, password) {
        const data = await this.request('/auth/login', {
            method: 'POST',
            auth: false,
            body: JSON.stringify({ email, password })
        });

        if (data && data.token) {
            this.token = data.token;
            localStorage.setItem('token', data.token);
            localStorage.setItem('user', JSON.stringify(data));
        }

        return data;
    }

    logout() {
        this.token = null;
        localStorage.removeItem('token');
        localStorage.removeItem('user');
    }

    // Get current user
    getCurrentUser() {
        const user = localStorage.getItem('user');
        return user ? JSON.parse(user) : null;
    }

    // Employees
    async getEmployees() {
        return await this.request('/employees');
    }

    async getEmployee(id) {
        return await this.request(`/employees/${id}`);
    }

    async createEmployee(data) {
        return await this.request('/employees', {
            method: 'POST',
            body: JSON.stringify(data)
        });
    }

    async updateEmployee(id, data) {
        return await this.request(`/employees/${id}`, {
            method: 'PUT',
            body: JSON.stringify(data)
        });
    }

    async deleteEmployee(id) {
        return await this.request(`/employees/${id}`, {
            method: 'DELETE'
        });
    }

    // Attendance
    async checkIn(employeeId, location) {
        return await this.request('/attendance/checkin', {
            method: 'POST',
            body: JSON.stringify({ employeeId, checkInLocation: location })
        });
    }

    async checkOut(employeeId, location) {
        return await this.request('/attendance/checkout', {
            method: 'POST',
            body: JSON.stringify({ employeeId, checkOutLocation: location })
        });
    }

    async getTodayAttendance() {
        return await this.request('/attendance/today');
    }

    async getAttendanceByDate(date) {
        return await this.request(`/attendance/date/${date}`);
    }

    async getEmployeeAttendance(employeeId, startDate, endDate) {
        let url = `/attendance/employee/${employeeId}`;
        if (startDate && endDate) {
            url += `?startDate=${startDate}&endDate=${endDate}`;
        }
        return await this.request(url);
    }

    // Meetings
    async getMeetings() {
        return await this.request('/meetings');
    }

    async getMyMeetings() {
        return await this.request('/meetings/my-meetings');
    }

    async getUpcomingMeetings() {
        return await this.request('/meetings/upcoming');
    }

    async getMeeting(id) {
        return await this.request(`/meetings/${id}`);
    }

    async createMeeting(data) {
        return await this.request('/meetings', {
            method: 'POST',
            body: JSON.stringify(data)
        });
    }

    async updateMeeting(id, data) {
        return await this.request(`/meetings/${id}`, {
            method: 'PUT',
            body: JSON.stringify(data)
        });
    }

    async deleteMeeting(id) {
        return await this.request(`/meetings/${id}`, {
            method: 'DELETE'
        });
    }

    async updateMeetingStatus(id, status) {
        return await this.request(`/meetings/${id}/status`, {
            method: 'PUT',
            body: JSON.stringify({ status })
        });
    }

    // Tasks
    async getTasks(employeeId = null, status = null) {
        let url = '/tasks';
        const params = [];
        if (employeeId) params.push(`employeeId=${employeeId}`);
        if (status) params.push(`status=${status}`);
        if (params.length) url += '?' + params.join('&');
        return await this.request(url);
    }

    async getMyTasks() {
        return await this.request('/tasks/my-tasks');
    }

    async getOverdueTasks() {
        return await this.request('/tasks/overdue');
    }

    async getTask(id) {
        return await this.request(`/tasks/${id}`);
    }

    async createTask(data) {
        return await this.request('/tasks', {
            method: 'POST',
            body: JSON.stringify(data)
        });
    }

    async updateTask(id, data) {
        return await this.request(`/tasks/${id}`, {
            method: 'PUT',
            body: JSON.stringify(data)
        });
    }

    async updateTaskStatus(id, status) {
        return await this.request(`/tasks/${id}/status`, {
            method: 'PUT',
            body: JSON.stringify({ status })
        });
    }

    async deleteTask(id) {
        return await this.request(`/tasks/${id}`, {
            method: 'DELETE'
        });
    }

    // Reports
    async getAttendanceReport(startDate, endDate, departmentId = null) {
        let url = `/reports/attendance?startDate=${startDate}&endDate=${endDate}`;
        if (departmentId) url += `&departmentId=${departmentId}`;
        return await this.request(url);
    }

    async getTaskAnalytics(startDate = null, endDate = null) {
        let url = '/reports/tasks';
        const params = [];
        if (startDate) params.push(`startDate=${startDate}`);
        if (endDate) params.push(`endDate=${endDate}`);
        if (params.length) url += '?' + params.join('&');
        return await this.request(url);
    }

    async getDailyAttendance(startDate, endDate) {
        return await this.request(`/reports/daily-attendance?startDate=${startDate}&endDate=${endDate}`);
    }

    async getEmployeePerformance(startDate, endDate) {
        return await this.request(`/reports/employee-performance?startDate=${startDate}&endDate=${endDate}`);
    }

    async exportAttendanceReport(startDate, endDate) {
        const url = `${this.baseURL}/reports/attendance/export?startDate=${startDate}&endDate=${endDate}`;
        window.open(url, '_blank');
    }

    async exportTaskAnalytics(startDate = null, endDate = null) {
        let url = `${this.baseURL}/reports/tasks/export`;
        const params = [];
        if (startDate) params.push(`startDate=${startDate}`);
        if (endDate) params.push(`endDate=${endDate}`);
        if (params.length) url += '?' + params.join('&');
        window.open(url, '_blank');
    }
}

// Create global API instance
const api = new APIClient();

// Utility Functions
const utils = {
    // Format date for display
    formatDate(dateString) {
        if (!dateString) return '-';
        const date = new Date(dateString);
        return date.toLocaleDateString('en-US', { year: 'numeric', month: 'short', day: 'numeric' });
    },

    // Format datetime for display
    formatDateTime(dateString) {
        if (!dateString) return '-';
        const date = new Date(dateString);
        return date.toLocaleString('en-US', { 
            year: 'numeric', 
            month: 'short', 
            day: 'numeric',
            hour: '2-digit',
            minute: '2-digit'
        });
    },

    // Format time only
    formatTime(dateString) {
        if (!dateString) return '-';
        const date = new Date(dateString);
        return date.toLocaleTimeString('en-US', { hour: '2-digit', minute: '2-digit' });
    },

    // Format date for input fields (YYYY-MM-DD)
    formatDateInput(date = new Date()) {
        const d = new Date(date);
        const year = d.getFullYear();
        const month = String(d.getMonth() + 1).padStart(2, '0');
        const day = String(d.getDate()).padStart(2, '0');
        return `${year}-${month}-${day}`;
    },

    // Format datetime for input fields (YYYY-MM-DDTHH:MM)
    formatDateTimeInput(date = new Date()) {
        const d = new Date(date);
        const year = d.getFullYear();
        const month = String(d.getMonth() + 1).padStart(2, '0');
        const day = String(d.getDate()).padStart(2, '0');
        const hours = String(d.getHours()).padStart(2, '0');
        const minutes = String(d.getMinutes()).padStart(2, '0');
        return `${year}-${month}-${day}T${hours}:${minutes}`;
    },

    // Get status badge class
    getStatusBadge(status) {
        const badges = {
            'Present': 'badge-success',
            'Completed': 'badge-success',
            'Accepted': 'badge-success',
            'Attended': 'badge-success',
            'Late': 'badge-warning',
            'In Progress': 'badge-info',
            'Pending': 'badge-info',
            'Invited': 'badge-info',
            'Absent': 'badge-danger',
            'Cancelled': 'badge-danger',
            'Declined': 'badge-danger',
            'Half-Day': 'badge-secondary'
        };
        return badges[status] || 'badge-secondary';
    },

    // Get priority badge class
    getPriorityBadge(priority) {
        const badges = {
            'Low': 'badge-info',
            'Medium': 'badge-warning',
            'High': 'badge-danger',
            'Urgent': 'badge-danger'
        };
        return badges[priority] || 'badge-secondary';
    },

    // Show alert
    showAlert(message, type = 'success', duration = 3000) {
        const alertDiv = document.createElement('div');
        alertDiv.className = `alert alert-${type}`;
        alertDiv.textContent = message;

        // Insert at top of content
        const content = document.querySelector('.content');
        if (content) {
            content.insertBefore(alertDiv, content.firstChild);

            // Auto remove after duration
            if (duration > 0) {
                setTimeout(() => alertDiv.remove(), duration);
            }
        }
    },

    // Show loading
    showLoading(element) {
        element.innerHTML = '<div class="loading"><div class="spinner"></div><p>Loading...</p></div>';
    },

    // Confirm dialog
    confirm(message) {
        return window.confirm(message);
    },

    // Check if user is logged in
    isAuthenticated() {
        return !!localStorage.getItem('token');
    },

    // Check if user has role
    hasRole(role) {
        const user = api.getCurrentUser();
        return user && user.role === role;
    },

    // Check if user is Admin or HR
    canManage() {
        return this.hasRole('Admin') || this.hasRole('HR');
    },

    // Redirect to login if not authenticated
    requireAuth() {
        if (!this.isAuthenticated()) {
            window.location.href = 'index.html';
        }
    }
};

// Auto redirect if not authenticated (for dashboard pages)
if (window.location.pathname !== '/index.html' && !window.location.pathname.endsWith('index.html')) {
    utils.requireAuth();
}
