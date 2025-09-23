# Vacancy Management System

A full-stack web application for managing grants and vacancies with categories. Built with .NET Core Web API backend and Angular frontend.

## 🚀 Features

- **Category Management**: Create, view, update, and delete categories
- **Grant Management**: Create, view, and manage grants with multiple categories
- **Filtering & Pagination**: Filter grants by category, country, and active status
- **Responsive UI**: Modern Angular frontend with Bootstrap styling
- **RESTful API**: Well-documented API endpoints with Swagger integration

## 🛠️ Tech Stack

### Backend (.NET Core 8)
- **Framework**: ASP.NET Core Web API
- **Database**: SQLite (Entity Framework Core)
- **Authentication**: JWT Bearer tokens (configured but disabled for demo)
- **Documentation**: Swagger/OpenAPI
- **Validation**: Data annotations and custom validators

### Frontend (Angular 17)
- **Framework**: Angular 17 with TypeScript
- **Styling**: CSS with responsive design
- **HTTP Client**: Angular HttpClient for API communication
- **Forms**: Reactive Forms with validation
- **Build Tool**: Angular CLI

## 📁 Project Structure

```
/
├── Vacancies/                 # .NET Core API
│   ├── Controllers/           # API Controllers
│   ├── Models/               # Data Models
│   ├── DTOs/                 # Data Transfer Objects
│   ├── Data/                 # Entity Framework Context
│   └── Program.cs            # Application entry point
├── front-end/front-end/      # Angular Application
│   ├── src/app/              # Angular Components & Services
│   ├── src/environments/     # Environment configurations
│   └── angular.json          # Angular CLI configuration
└── README.md
```

## 🚀 Getting Started

### Prerequisites
- .NET 8.0 SDK
- Node.js (v18 or later)
- npm or yarn

### Backend Setup

1. **Navigate to the API directory:**
   ```bash
   cd Vacancies
   ```

2. **Restore NuGet packages:**
   ```bash
   dotnet restore
   ```

3. **Build the project:**
   ```bash
   dotnet build
   ```

4. **Run the API:**
   ```bash
   dotnet run --urls="http://localhost:5000"
   ```

The API will be available at `http://localhost:5000` with Swagger documentation at `http://localhost:5000/swagger`.

### Frontend Setup

1. **Navigate to the frontend directory:**
   ```bash
   cd front-end/front-end
   ```

2. **Install dependencies:**
   ```bash
   npm install
   ```

3. **Start the development server:**
   ```bash
   npm start
   ```

The frontend will be available at `http://localhost:4200`.

## 📊 Database

The application uses SQLite for data storage with Entity Framework Core. The database is automatically created on first run with the following tables:

- **Categories**: Store grant categories
- **Grants**: Store grant information
- **GrantCategories**: Many-to-many relationship table

## 🔧 API Endpoints

### Categories
- `GET /api/categories` - Get paginated categories
- `POST /api/categories` - Create new category
- `PUT /api/categories/{id}` - Update category
- `DELETE /api/categories/{id}` - Delete category

### Grants
- `GET /api/grants` - Get paginated grants with filtering
- `POST /api/grants` - Create new grant

### Query Parameters
- `page` - Page number (default: 1)
- `pageSize` - Items per page (default: 10)
- `categoryId` - Filter by category
- `country` - Filter by country
- `activeOnly` - Show only active grants

## 🧪 Testing

### Sample API Calls

**Create a Category:**
```bash
curl -X POST http://localhost:5000/api/categories \
  -H "Content-Type: application/json" \
  -d '{"name":"Technology","description":"Technology and innovation grants"}'
```

**Create a Grant:**
```bash
curl -X POST http://localhost:5000/api/grants \
  -H "Content-Type: application/json" \
  -d '{
    "title":"AI Innovation Grant",
    "description":"Grant for artificial intelligence research",
    "country":"United States",
    "deadline":"2025-12-31T23:59:59Z",
    "requirements":"PhD in Computer Science",
    "fundingAmount":"$100,000",
    "categoryIds":["category-guid-here"]
  }'
```

## 🔒 Security Features

- Input validation on all endpoints
- CORS configuration for frontend integration
- JWT authentication framework (ready for implementation)
- SQL injection protection via Entity Framework
- Custom validation attributes (e.g., FutureDateAttribute)

## 🐛 Recent Fixes

### Fixed Issues:
1. **Angular Template Syntax Error**: Fixed parentheses mismatch in event binding
2. **CSS Bundle Size**: Increased Angular budget limits for component styles
3. **Nullable Reference Warnings**: Added proper null handling in .NET models
4. **Database Configuration**: Migrated from SQL Server to SQLite for cross-platform compatibility
5. **CORS Configuration**: Properly configured for Angular frontend
6. **Authorization**: Temporarily disabled for demo purposes

## 📝 Development Notes

- The application automatically creates the database on startup
- Authorization is currently disabled for testing purposes
- The frontend API URL is configured in `environments/environment.ts`
- All API endpoints return paginated results with metadata

## 🔮 Future Enhancements

- [ ] Implement JWT authentication
- [ ] Add user roles and permissions
- [ ] Email notifications for grant deadlines
- [ ] File upload for grant documents
- [ ] Advanced search and filtering
- [ ] Grant application workflow
- [ ] Dashboard with analytics
- [ ] Export functionality (PDF, Excel)

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests if applicable
5. Submit a pull request

## 📞 Support

For issues and questions, please create an issue in the GitHub repository.
