# HNTD API

A RESTful API for the HNTD (Haunted Location Finder) application. Built with C# ASP.NET Core and PostgreSQL.

## Technologies

- C# / ASP.NET Core 8
- Entity Framework Core
- PostgreSQL
- ASP.NET Identity (Cookie Authentication)
- Backblaze B2 (Image Storage)

## Features

- User authentication (register, login, logout, session management)
- User profile management with image uploads
- Haunted Locations CRUD with ownership verification
- Favorites system
- Filter data (location types, activity levels, paranormal activities)

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [PostgreSQL](https://www.postgresql.org/download/)
- [Backblaze B2 Account](https://www.backblaze.com/b2/cloud-storage.html) (for image uploads)

## Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/yourusername/HntdApi.git
cd HntdApi
```

### 2. Set Up the Database

Make sure PostgreSQL is running. Create a database named `HntdApi` or update the connection string to match your database name.

### 3. Configure User Secrets

This project uses .NET User Secrets for sensitive configuration. Initialize and set the required secrets:

```bash
dotnet user-secrets init
dotnet user-secrets set "HntdApiDbConnectionString" "Host=localhost;Database=HntdApi;Username=your_username;Password=your_password"
dotnet user-secrets set "B2:KeyId" "your_backblaze_key_id"
dotnet user-secrets set "B2:ApplicationKey" "your_backblaze_application_key"
dotnet user-secrets set "B2:BucketId" "your_backblaze_bucket_id"
dotnet user-secrets set "B2:BucketName" "your_backblaze_bucket_name"
```

**Note:** Replace the placeholder values with your actual PostgreSQL credentials and Backblaze B2 credentials.

#### Setting Up Backblaze B2

1. Create a free account at [backblaze.com](https://www.backblaze.com)
2. Create a new bucket (set to **Public** for image access)
3. Create an Application Key with read/write access to your bucket
4. Note your Key ID, Application Key, Bucket ID, and Bucket Name

### 4. Run Database Migrations

```bash
dotnet ef database update
```

This will create all necessary tables and seed initial data (location types, activity levels, paranormal activities).

### 5. Run the API

```bash
dotnet run
```

The API will start on `http://localhost:5104`.

## API Endpoints

### Authentication

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/auth/login` | Login with Basic Auth header |
| GET | `/api/auth/logout` | Logout current user |
| GET | `/api/auth/me` | Get current user info |
| POST | `/api/auth/register` | Register new user |
| PUT | `/api/auth/profile` | Update user profile |

### Haunted Locations

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/hauntedlocations` | Get all locations |
| GET | `/api/hauntedlocations/{id}` | Get location by ID |
| POST | `/api/hauntedlocations` | Create new location |
| PUT | `/api/hauntedlocations/{id}` | Update location (owner only) |
| DELETE | `/api/hauntedlocations/{id}` | Delete location (owner only) |

### Favorites

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/favorites` | Get current user's favorites |
| POST | `/api/favorites` | Add location to favorites |
| DELETE | `/api/favorites/{locationId}` | Remove from favorites |

### Filter Data

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/locationtypes` | Get all location types |
| GET | `/api/activitylevels` | Get all activity levels |
| GET | `/api/activities` | Get all paranormal activities |

### Image Upload

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/image/upload` | Upload image to Backblaze B2 |

## CORS Configuration

The API is configured to accept requests from `http://localhost:3000` (the React frontend). Update `Program.cs` if your frontend runs on a different port.

## Troubleshooting

### Database Connection Issues
- Verify PostgreSQL is running
- Check your connection string in user secrets
- Ensure the database exists

### Image Upload Issues
- Verify Backblaze B2 credentials are correct
- Ensure your bucket is set to Public
- Check that your Application Key has read/write permissions

### CORS Errors
- Make sure the frontend URL is in the allowed origins in `Program.cs`
- Verify `credentials: "include"` is set in frontend fetch requests