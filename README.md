# ASP.NET Core Todo List (MVC + Web API)

Full-stack ASP.NET Core todo list application with a Razor MVC web UI and a RESTful Web API backend. It uses EF Core with SQLite for persistence, ASP.NET Core Identity for authentication, and supports CRUD for lists and tasks, task assignment, search by multiple criteria, and tag-based organization.

## Key features

- **Authentication**: user registration/login/logout (ASP.NET Core Identity)
- **Todo lists**: create, edit, delete, view your lists
- **Tasks**: create, edit, delete, view details; due dates and status
- **Assigned tasks**: view tasks assigned to you, filter and sort, change status
- **Search**: find tasks by title and date ranges (created/due)
- **Tags**: browse all tags, view tasks by tag, tags visible on task details

## Tech stack

- **.NET**: ASP.NET Core (.NET 6)
- **UI**: Razor Views (MVC) + Bootstrap 5
- **API**: ASP.NET Core Web API
- **Data**: Entity Framework Core + SQLite
- **Auth**: ASP.NET Core Identity (WebApp) + API key (WebApi)

## Architecture overview

This solution contains two applications:

- **`TodoListApp.WebApp`**: MVC web application (browser UI) + Identity user store (`users.db`)
- **`TodoListApp.WebApi`**: REST API backend + domain data store for lists/tasks/tags (`todolist.db`)

The WebApp communicates with the WebApi via typed `HttpClient` services and sends an API key in the `Authorization: Bearer <key>` header.

## Screenshots

Place your screenshots under:

- **`docs/screenshots/`**

Then update the links below (replace `TODO:` with real filenames you add):

- **Login / Register**: `docs/screenshots/TODO-login.png`, `docs/screenshots/TODO-register.png`
- **My Lists**: `docs/screenshots/TODO-lists.png`
- **Tasks in a List**: `docs/screenshots/TODO-tasks.png`
- **Task Details (with tags)**: `docs/screenshots/TODO-task-details.png`
- **Assigned Tasks**: `docs/screenshots/TODO-assigned-tasks.png`
- **Search**: `docs/screenshots/TODO-search.png`
- **Tags**: `docs/screenshots/TODO-tags.png`, `docs/screenshots/TODO-tasks-by-tag.png`

## How to run locally

### Prerequisites

- .NET SDK 6

### 1) Run the Web API

- Start **`TodoListApp.WebApi`** (it runs on `https://localhost:7001` by default).

### 2) Run the Web App

- Start **`TodoListApp.WebApp`** and open the URL shown in the console.

### 3) First-time database setup

This repo contains EF Core migrations for both applications. If the databases are not created yet, apply migrations:

- WebApi migrations create `todolist.db`
- WebApp migrations create `users.db`

## Configuration

- **WebApp** (`TodoListApp.WebApp/appsettings.json`)
  - `WebApi:BaseUrl` (default: `https://localhost:7001/`)
  - `WebApi:ApiKey`
  - `ConnectionStrings:UsersDb` (default SQLite file: `users.db`)

- **WebApi** (`TodoListApp.WebApi/appsettings.json`)
  - `Authentication:ApiKey`
  - `ConnectionStrings:TodoListDb` (default SQLite file: `todolist.db`)

## Project structure

```
TodoListApp.WebApp/   # MVC UI + Identity (users.db)
TodoListApp.WebApi/   # REST API + EF Core (todolist.db)
docs/screenshots/     # screenshots for README
```
