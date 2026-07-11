# 🎯 Interview Prep Platform

A full-stack web application designed to help developers systematically track their interview preparation journey — built with ASP.NET Core 9 and React.

![Dashboard](https://img.shields.io/badge/Status-Active-brightgreen)
![.NET](https://img.shields.io/badge/.NET-9.0-purple)
![React](https://img.shields.io/badge/React-18-blue)
![License](https://img.shields.io/badge/License-MIT-yellow)

---

## 🚀 Live Demo
https://interview-prep-kohl.vercel.app
Rilway+vercel

---

## 💡 Why This Project?

Most students build Student Management Systems or Library Systems.

This project is different — it solves a **real problem I personally face**: tracking hundreds of DSA problems, contest ratings, and study notes in one place, with AI-powered explanations on demand.

---

## ✨ Features

### 🔐 Authentication
- JWT-based Register / Login / Logout
- BCrypt password hashing
- Role-based authorization
- Protected routes on frontend

### 📊 Dashboard
- Total problems solved (Easy / Medium / Hard)
- Topic breakdown — Pie chart
- Monthly progress — Line chart
- Current streak and longest streak
- Active goals with progress bars

### 💻 DSA Problem Tracker
- Log solved problems (LeetCode, CodeForces, HackerRank, etc.)
- Filter by difficulty, topic, platform
- Search by title
- Pagination (20 problems per page)
- Add notes and problem URL

### 📝 Notes Module
- Create topic-wise notes
- View, edit, delete notes
- Clean two-panel layout

### 🏆 Contest Tracker
- Track contest rank and rating
- See rating change (+35, -20)
- Total rating change summary
- Best rank achieved

### 🎯 Goal Tracking
- Set custom goals (e.g. Solve 300 problems)
- Update progress manually
- Progress bar with percentage
- Completed goals section

### 🔥 Streak System
- Tracks consecutive days of solving
- Current streak and longest streak
- Solved today indicator

### 🤖 AI Assistant (Powered by Groq/Llama 3.3)
- Select any DSA topic
- Get instant AI explanation with:
  - Definition
  - Key concepts
  - Time and space complexity
  - Example with step-by-step walkthrough
  - Common interview questions
  - Tips and tricks

### 🎤 Mock Interview Generator
- Choose topic and difficulty
- AI generates realistic interview questions
- Includes examples, constraints, hints
- Built-in interview timer

### 📄 Resume Analyzer
- Paste your resume text
- AI detects your skills
- Identifies missing skills
- Gives improvement suggestions
- Overall rating out of 10

---

## 🛠️ Tech Stack

### Backend
| Technology | Purpose |
|---|---|
| ASP.NET Core 9 | REST API framework |
| Entity Framework Core | ORM for database |
| SQL Server | Database |
| JWT Bearer | Authentication |
| BCrypt.Net | Password hashing |
| Groq API (Llama 3.3) | AI features |

### Frontend
| Technology | Purpose |
|---|---|
| React 18 | UI framework |
| Vite | Build tool |
| Tailwind CSS | Styling |
| Recharts | Charts and graphs |
| Axios | HTTP client |
| React Router | Client-side routing |

---

## 🏗️ Architecture
InterviewPrep/
├── InterviewPrep.API/          ← .NET Backend
│   ├── Controllers/            ← HTTP endpoints
│   │   ├── AuthController
│   │   ├── ProblemsController
│   │   ├── NotesController
│   │   ├── ContestsController
│   │   ├── GoalsController
│   │   ├── DashboardController
│   │   ├── AIController
│   │   └── ProfileController
│   ├── Services/               ← Business logic
│   │   ├── AuthService
│   │   ├── ProblemService
│   │   ├── NoteService
│   │   ├── ContestService
│   │   ├── GoalService
│   │   ├── DashboardService
│   │   └── AIService
│   ├── Entities/               ← Database models
│   ├── DTOs/                   ← Request/Response shapes
│   ├── Data/                   ← DbContext
│   └── Helpers/                ← JWT helper
│
└── frontend/                   ← React Frontend
└── src/
├── pages/
│   ├── Dashboard
│   ├── Problems
│   ├── Notes
│   ├── Contests
│   ├── Goals
│   ├── AIAssistant
│   ├── MockInterview
│   └── ResumeAnalyzer
├── components/
├── context/            ← AuthContext
└── services/           ← API calls

---

## 🗄️ Database Schema
Users (1)
├──< Problems    (UserId FK)
├──< Notes       (UserId FK)
├──< Contests    (UserId FK)
├──< Goals       (UserId FK)
└──1 Streak      (UserId FK)

---

## ⚙️ Setup and Installation

### Prerequisites
- .NET 9 SDK
- Node.js 18+
- SQL Server (LocalDB works)
- Groq API Key (free at console.groq.com)

### Backend Setup

```bash
# Clone the repository
git clone https://github.com/Shivamani1913/InterviewPrep.git
cd InterviewPrep/InterviewPrep.API

# Copy example config
cp appsettings.example.json appsettings.json

# Edit appsettings.json and add your keys:
# - SQL Server connection string
# - JWT secret key
# - Groq API key

# Install dependencies and run migrations
dotnet restore
dotnet ef database update

# Start the API
dotnet run
# API runs on http://localhost:5116
# Swagger UI at http://localhost:5116/swagger
```

### Frontend Setup

```bash
cd ../frontend

# Install dependencies
npm install

# Start the dev server
npm run dev
# App runs on http://localhost:5173
```

---

## 🔑 Environment Configuration

Copy `appsettings.example.json` to `appsettings.json` and fill in:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "your-sql-server-connection-string"
  },
  "Jwt": {
    "SecretKey": "your-secret-key-min-32-characters",
    "Issuer": "InterviewPrepAPI",
    "Audience": "InterviewPrepApp",
    "ExpiryHours": "24"
  },
  "Groq": {
    "ApiKey": "your-groq-api-key"
  }
}
```

---

## 📡 API Endpoints
Auth
POST   /api/auth/register
POST   /api/auth/login
GET    /api/auth/me
POST   /api/auth/logout
PUT    /api/profile
Problems
GET    /api/problems         (with filter, search, pagination)
POST   /api/problems
GET    /api/problems/{id}
PUT    /api/problems/{id}
DELETE /api/problems/{id}
Notes
GET    /api/notes
POST   /api/notes
PUT    /api/notes/{id}
DELETE /api/notes/{id}
Contests
GET    /api/contests
POST   /api/contests
DELETE /api/contests/{id}
Goals
GET    /api/goals
POST   /api/goals
PUT    /api/goals/{id}/progress
DELETE /api/goals/{id}
Dashboard
GET    /api/dashboard
AI
POST   /api/ai/explain
POST   /api/ai/mock-interview
POST   /api/ai/analyze-resume

---

## 🔒 Security Features

- JWT token authentication on all protected endpoints
- BCrypt password hashing (cost factor 11)
- User data isolation — users can only access their own data
- API key stored in environment variables (never in code)
- CORS configured for specific frontend origin

---

## 👨‍💻 Developer

**Shivamani Kurra**
- GitHub: [@Shivamani1913](https://github.com/Shivamani1913)

---

## 📄 License

This project is open source and available under the [MIT License](LICENSE).
