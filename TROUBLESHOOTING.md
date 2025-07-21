# 🔧 Troubleshooting Guide

## Quick Start (Recommended)

### Option 1: Use the Scripts
```bash
# Terminal 1 - Start Backend
chmod +x start-backend.sh
./start-backend.sh

# Terminal 2 - Start Frontend  
chmod +x start-frontend.sh
./start-frontend.sh
```

### Option 2: Manual Start

**Backend:**
```bash
cd Vacancies
dotnet restore
dotnet ef database update
dotnet run
```

**Frontend:**
```bash
cd front-end/front-end
npm install
npm start
```

## Common Issues & Solutions

### 🔴 Backend Issues

#### 1. Database Connection Error
**Error:** `Cannot connect to LocalDB`
**Solution:** I've updated the project to use SQLite instead
- ✅ Updated `appsettings.json` to use SQLite
- ✅ Updated `Vacancies.csproj` to use SQLite package
- ✅ Updated `Program.cs` to use SQLite

#### 2. Missing Entity Framework Tools
**Error:** `dotnet ef command not found`
**Solution:**
```bash
dotnet tool install --global dotnet-ef
```

#### 3. Migration Issues
**Error:** `No migrations found`
**Solution:**
```bash
cd Vacancies
dotnet ef migrations add InitialCreate
dotnet ef database update
```

#### 4. CORS Issues
**Error:** `CORS policy error`
**Solution:** Already fixed in `Program.cs` - CORS is configured for `http://localhost:4200`

#### 5. Port Already in Use
**Error:** `Address already in use`
**Solution:**
```bash
# Kill process on port 7080
lsof -ti:7080 | xargs kill -9

# Or change port in launchSettings.json
```

### 🔴 Frontend Issues

#### 1. Node Modules Missing
**Error:** `Cannot resolve dependency`
**Solution:**
```bash
cd front-end/front-end
rm -rf node_modules package-lock.json
npm install
```

#### 2. Angular CLI Missing
**Error:** `ng command not found`
**Solution:**
```bash
npm install -g @angular/cli@17
```

#### 3. TypeScript Errors
**Error:** `Property does not exist on type`
**Solution:** The components are properly typed, but if you see errors:
```bash
cd front-end/front-end
npm run build
```

#### 4. API Connection Failed
**Error:** `Http failure response`
**Solution:** 
- ✅ Make sure backend is running on `https://localhost:7080`
- ✅ Check browser console for CORS errors
- ✅ Environment files are configured correctly

#### 5. Port Already in Use
**Error:** `Port 4200 is already in use`
**Solution:**
```bash
# Kill process on port 4200
lsof -ti:4200 | xargs kill -9

# Or use different port
ng serve --port 4201
```

## Testing the Connection

### 1. Test Backend
```bash
# Check if API is running
curl https://localhost:7080/api/categories

# Or visit in browser:
https://localhost:7080/swagger
```

### 2. Test Frontend
```bash
# Visit in browser:
http://localhost:4200
```

## Application URLs

- **Frontend:** http://localhost:4200
- **Backend API:** https://localhost:7080
- **Swagger UI:** https://localhost:7080/swagger
- **Categories:** http://localhost:4200/categories
- **Grants:** http://localhost:4200/grants

## What Should Work

### Backend Endpoints:
- ✅ `GET /api/categories` - Get categories with pagination and search
- ✅ `POST /api/categories` - Create new category
- ✅ `GET /api/grants` - Get grants with filters
- ✅ `POST /api/grants` - Create new grant

### Frontend Features:
- ✅ Categories management with search and pagination
- ✅ Grants management with filtering and creation
- ✅ Responsive design
- ✅ Form validation
- ✅ Error handling

## Still Having Issues?

1. **Check Prerequisites:**
   - .NET 8 SDK installed
   - Node.js 16+ installed
   - npm 8+ installed

2. **Clean Everything:**
   ```bash
   # Backend
   cd Vacancies
   dotnet clean
   rm -rf bin obj
   
   # Frontend
   cd front-end/front-end
   rm -rf node_modules package-lock.json dist
   ```

3. **Start Fresh:**
   ```bash
   # Backend
   cd Vacancies
   dotnet restore
   dotnet ef database drop --force
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   dotnet run
   
   # Frontend
   cd front-end/front-end
   npm install
   npm start
   ```

## Database Schema

The application will create these tables:
- `Categories` - Category information
- `Grants` - Grant information  
- `GrantCategories` - Many-to-many relationship

## Need Help?

If you're still having issues, please share:
1. The exact error message
2. Which step failed (backend or frontend)
3. Your operating system
4. Node.js and .NET versions