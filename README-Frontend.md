# Grant Management System - Frontend

This is an Angular application that provides a user interface for managing grants and categories. It connects to a .NET backend API.

## Features

- **Categories Management**: View, search, and create categories
- **Grants Management**: View, filter, and create grants
- **Responsive Design**: Works on desktop and mobile devices
- **Real-time Data**: Connects to backend API for real-time data

## Prerequisites

- Node.js (v16 or higher)
- npm (v8 or higher)
- Angular CLI (v17 or higher)

## Installation

1. Navigate to the frontend directory:
```bash
cd front-end/front-end
```

2. Install dependencies:
```bash
npm install
```

## Configuration

The application is configured to connect to the backend API at `https://localhost:7080/api`. 

### Environment Configuration

- **Development**: `src/environments/environment.ts` - Points to `https://localhost:7080/api`
- **Production**: `src/environments/environment.prod.ts` - Update with your production API URL

## Running the Application

### Development Server

```bash
npm start
```

The application will be available at `http://localhost:4200`

### Build for Production

```bash
npm run build
```

## Backend Connection

The frontend connects to the following backend endpoints:

### Categories API
- `GET /api/categories` - Get paginated categories with search
- `POST /api/categories` - Create new category

### Grants API
- `GET /api/grants` - Get paginated grants with filters
- `POST /api/grants` - Create new grant

## Features Overview

### Categories Page (`/categories`)
- View all categories in a paginated grid
- Search categories by name or description
- Create new categories with form validation
- Responsive card-based layout

### Grants Page (`/grants`)
- View all grants in a detailed list format
- Filter by category, country, and active status
- Create new grants with comprehensive form
- Visual indicators for expired grants
- Category tags for easy identification

## Project Structure

```
src/
├── app/
│   ├── components/
│   │   ├── categories/
│   │   │   ├── categories.component.ts
│   │   │   ├── categories.component.html
│   │   │   └── categories.component.css
│   │   └── grants/
│   │       ├── grants.component.ts
│   │       ├── grants.component.html
│   │       └── grants.component.css
│   ├── services/
│   │   ├── category.service.ts
│   │   └── grant.service.ts
│   ├── app-routing.module.ts
│   ├── app.component.ts
│   ├── app.component.html
│   ├── app.component.css
│   └── app.module.ts
├── environments/
│   ├── environment.ts
│   └── environment.prod.ts
└── styles.css
```

## Backend Requirements

Make sure your .NET backend is running and has CORS configured to allow requests from `http://localhost:4200`.

The backend should be running on `https://localhost:7080` with the following endpoints available:
- `/api/categories`
- `/api/grants`

## Troubleshooting

### CORS Issues
If you encounter CORS errors, make sure your .NET backend has CORS configured properly:

```csharp
// In Program.cs
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowedOrigins", policy =>
    {
        policy.WithOrigins("http://localhost:4200", "https://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// ...

app.UseCors("AllowedOrigins");
```

### API Connection Issues
1. Verify the backend is running on `https://localhost:7080`
2. Check the console for any HTTP errors
3. Verify the API endpoints are responding correctly
4. Check if SSL certificates are properly configured

## Technologies Used

- **Angular 17**: Frontend framework
- **TypeScript**: Programming language
- **RxJS**: Reactive programming
- **Angular Router**: Navigation
- **Angular Forms**: Form handling and validation
- **HttpClient**: HTTP communication
- **CSS Grid/Flexbox**: Layout and responsive design

## Browser Support

- Chrome (latest)
- Firefox (latest)
- Safari (latest)
- Edge (latest)