#!/bin/bash

echo "🚀 Starting Backend..."
echo "📁 Navigating to Vacancies directory..."
cd Vacancies

echo "📦 Restoring packages..."
dotnet restore

echo "🗄️  Creating/updating database..."
dotnet ef database update

echo "🔥 Starting the API server..."
echo "Backend will be available at: https://localhost:7080"
echo "Swagger UI will be available at: https://localhost:7080/swagger"
echo ""
dotnet run