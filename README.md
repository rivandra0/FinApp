# Project Name

The application is the simple app with authentication and authorization made with NET CORE 9.0 MVC.

## Features

- Jwt for authentication and authorization.
- Two additional layer of
    - Role Based Authorization.
    - License Based Authorization.

## Prerequisites

Before you begin, ensure you have met the following requirements:

- **.NET SDK**: This project use NET CORE 9.0.
- **Database**: This project uses SQL Server Express.

## Installation

Follow these steps to install and run the application:

### 1. Clone the repository

### 2. Restore dependencies

Restore NuGet packages:

```bash
dotnet restore
```

### 3. Configure the database

- Update the `appsettings.json` file in the root directory with your database connection string:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=your_server;Database=your_database;User Id=your_user;Password=your_password;"
}
"JwtSettings": { "TokenSecret": "dangerous to be here" },
```

- Go to SQL Server then run the Sql Script attached on the email to build the database and all of it's tables:


### 4. Build the application

Compile the application to ensure everything is set up correctly:

```bash
dotnet build
```

### 5. Run the application

Start the application locally:

```bash
dotnet run
```

By default, the application will be available at `http://localhost:5000`.







