@echo off
echo 🚀 Starting Vacancy Management System...

:: Check if .NET is available
dotnet --version >nul 2>&1
if %errorlevel% neq 0 (
    echo ❌ .NET Core SDK not found. Please install .NET 8.0 SDK
    pause
    exit /b 1
)

:: Check if npm is available
npm --version >nul 2>&1
if %errorlevel% neq 0 (
    echo ❌ npm not found. Please install Node.js and npm
    pause
    exit /b 1
)

:: Start API
echo 📡 Starting .NET Core API...
cd Vacancies
start "API Server" cmd /k "dotnet restore && dotnet build && dotnet run --urls=http://localhost:5000"
cd ..

:: Wait a moment for API to start
timeout /t 5 /nobreak >nul

:: Start Frontend
echo 🎨 Starting Angular Frontend...
cd front-end\front-end
if not exist node_modules (
    echo 📦 Installing npm dependencies...
    npm install
)
start "Angular Frontend" cmd /k "npm start"
cd ..\..

echo.
echo 🎉 Vacancy Management System is starting!
echo.
echo 📱 Frontend: http://localhost:4200
echo 🔧 API: http://localhost:5000
echo 📚 API Documentation: http://localhost:5000/swagger
echo.
echo ✨ Sample API endpoints to test:
echo    GET  http://localhost:5000/api/categories
echo    GET  http://localhost:5000/api/grants
echo    POST http://localhost:5000/api/categories
echo.
echo 🛑 Close the opened command windows to stop the applications
echo.
pause