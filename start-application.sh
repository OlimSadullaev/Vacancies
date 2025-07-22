#!/bin/bash

# Vacancy Management System - Startup Script
echo "🚀 Starting Vacancy Management System..."

# Function to check if a port is in use
check_port() {
    if lsof -Pi :$1 -sTCP:LISTEN -t >/dev/null ; then
        echo "⚠️  Port $1 is already in use"
        return 1
    else
        return 0
    fi
}

# Function to start the API
start_api() {
    echo "📡 Starting .NET Core API..."
    cd Vacancies
    
    # Check if dotnet is available
    if ! command -v dotnet &> /dev/null; then
        echo "❌ .NET Core SDK not found. Please install .NET 8.0 SDK"
        exit 1
    fi
    
    # Restore packages and build
    dotnet restore
    dotnet build
    
    if [ $? -eq 0 ]; then
        echo "✅ API build successful"
        # Start API in background
        nohup dotnet run --urls="http://localhost:5000" > api.log 2>&1 &
        API_PID=$!
        echo "🌐 API started on http://localhost:5000 (PID: $API_PID)"
        echo "📚 Swagger documentation available at http://localhost:5000/swagger"
    else
        echo "❌ API build failed"
        exit 1
    fi
    
    cd ..
}

# Function to start the frontend
start_frontend() {
    echo "🎨 Starting Angular Frontend..."
    cd front-end/front-end
    
    # Check if npm is available
    if ! command -v npm &> /dev/null; then
        echo "❌ npm not found. Please install Node.js and npm"
        exit 1
    fi
    
    # Install dependencies if node_modules doesn't exist
    if [ ! -d "node_modules" ]; then
        echo "📦 Installing npm dependencies..."
        npm install
    fi
    
    # Start frontend in background
    nohup npm start > angular.log 2>&1 &
    FRONTEND_PID=$!
    echo "🌐 Frontend starting on http://localhost:4200 (PID: $FRONTEND_PID)"
    
    cd ../..
}

# Function to wait for services to be ready
wait_for_services() {
    echo "⏳ Waiting for services to start..."
    
    # Wait for API
    for i in {1..30}; do
        if curl -s http://localhost:5000/api/categories > /dev/null 2>&1; then
            echo "✅ API is ready"
            break
        fi
        if [ $i -eq 30 ]; then
            echo "❌ API failed to start within 30 seconds"
            exit 1
        fi
        sleep 1
    done
    
    # Wait for frontend
    for i in {1..60}; do
        if curl -s http://localhost:4200 > /dev/null 2>&1; then
            echo "✅ Frontend is ready"
            break
        fi
        if [ $i -eq 60 ]; then
            echo "❌ Frontend failed to start within 60 seconds"
            exit 1
        fi
        sleep 1
    done
}

# Main execution
main() {
    # Check if ports are available
    if ! check_port 5000; then
        echo "❌ Cannot start API - port 5000 is in use"
        exit 1
    fi
    
    if ! check_port 4200; then
        echo "❌ Cannot start Frontend - port 4200 is in use"
        exit 1
    fi
    
    # Start services
    start_api
    sleep 3
    start_frontend
    
    # Wait for services to be ready
    wait_for_services
    
    echo ""
    echo "🎉 Vacancy Management System is now running!"
    echo ""
    echo "📱 Frontend: http://localhost:4200"
    echo "🔧 API: http://localhost:5000"
    echo "📚 API Documentation: http://localhost:5000/swagger"
    echo ""
    echo "📝 Logs:"
    echo "   API: ./Vacancies/api.log"
    echo "   Frontend: ./front-end/front-end/angular.log"
    echo ""
    echo "🛑 To stop the applications:"
    echo "   pkill -f dotnet"
    echo "   pkill -f ng"
    echo ""
    echo "✨ Sample API endpoints to test:"
    echo "   GET  http://localhost:5000/api/categories"
    echo "   GET  http://localhost:5000/api/grants"
    echo "   POST http://localhost:5000/api/categories"
    echo ""
}

# Handle Ctrl+C
trap 'echo "🛑 Stopping services..."; pkill -f dotnet; pkill -f ng; exit 0' INT

# Run main function
main

# Keep script running
echo "Press Ctrl+C to stop all services..."
while true; do
    sleep 10
done