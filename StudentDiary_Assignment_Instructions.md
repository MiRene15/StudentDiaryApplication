# Student Diary Application - Assignment Instructions

## Overview
You have been provided with a zip file containing the complete Student Diary application with layered architecture. Your task is to create a Git repository and commit each project layer following proper version control practices.

## Learning Objectives
- Practice proper Git version control workflows
- Understand layered architecture project structure
- Learn to create meaningful commit messages following user story format
- Practice incremental development simulation

---

## Prerequisites
- Git installed on your machine
- GitHub account (or GitLab/Bitbucket)
- .NET SDK installed
- Basic understanding of Git commands

---

## Assignment Instructions

### Step 1: Extract and Examine the Project Structure

1. **Extract the provided zip file** to a working directory
2. **Examine the project structure** - you should see:
   ```
   StudentDiary/
   ├── StudentDiary.sln
   ├── StudentDiary.Infrastructure/
   ├── StudentDiary.Services/
   └── StudentDiary.Presentation/
   ```

### Step 2: Create GitHub Repository

1. **Create a new repository** on GitHub:
   - Repository name: `StudentDiary-[YourName]` (e.g., `StudentDiary-JohnDoe`)
   - Description: "Student Diary Application with Layered Architecture"
   - Set to **Public**
   - **DO NOT** initialize with README, .gitignore, or license
   - Copy the repository URL for later use

### Step 3: Initialize Local Git Repository

Navigate to your extracted StudentDiary folder and run:

```bash
# Initialize Git repository
git init

# Add remote origin (replace with your repository URL)
git remote add origin https://github.com/yourusername/StudentDiary-YourName.git

# Create and switch to main branch
git branch -M main
```

### Step 4: Create .gitignore File

Create a `.gitignore` file in the root directory with the following content:

```gitignore
# Build results
[Dd]ebug/
[Dd]ebugPublic/
[Rr]elease/
[Rr]eleases/
x64/
x86/
[Aa][Rr][Mm]/
[Aa][Rr][Mm]64/
bld/
[Bb]in/
[Oo]bj/
[Ll]og/

# User-specific files
*.rsuser
*.suo
*.user
*.userosscache
*.sln.docstates

# Visual Studio files
.vs/

# NuGet packages
*.nupkg
*.snupkg
.nuget/

# Database files
*.db
*.db-shm
*.db-wal

# OS generated files
.DS_Store
Thumbs.db
```

**Commit the .gitignore:**
```bash
git add .gitignore
git commit -m "chore: Add .gitignore file"
```

### Step 5: Commit Each Layer Incrementally

You must commit each project layer separately following the user story format. **IMPORTANT:** Only commit ONE project at a time.

#### Commit 1: Infrastructure Layer
```bash
# Add only Infrastructure project files
git add StudentDiary.Infrastructure/
git add StudentDiary.sln

# Commit with proper message
git commit -m "feat: US001 - Add Infrastructure layer with Entity Models and DbContext

- Created User entity with authentication properties
- Created DiaryEntry entity with rich text content support
- Implemented StudentDiaryContext with proper relationships
- Added Entity Framework Core configuration
- Set up SQLite database provider"
```

#### Commit 2: Services Layer
```bash
# Add only Services project files
git add StudentDiary.Services/

# Commit with proper message
git commit -m "feat: US002 - Add Services layer with DTOs and Interfaces

- Created UserDTOs for authentication operations (Register, Login, Profile)
- Created DiaryEntryDTOs for CRUD operations
- Implemented IAuthService interface for user management
- Implemented IDiaryService interface for diary operations
- Added password reset and account lockout DTOs"
```

#### Commit 3: Presentation Layer (Basic Structure)
```bash
# Add only Presentation project files (without custom controllers/views yet)
git add StudentDiary.Presentation/

# Commit with proper message
git commit -m "feat: US003 - Add Presentation layer with MVC configuration

- Created ASP.NET Core MVC project structure
- Configured Entity Framework with SQLite connection
- Added session management configuration
- Set up dependency injection for services
- Configured middleware pipeline for authentication"
```

#### Commit 4: Database Migration
```bash
# Add migration files if not already included
git add StudentDiary.Infrastructure/Migrations/

# Commit with proper message
git commit -m "feat: US004 - Add initial database migration

- Generated InitialCreate migration with User and DiaryEntry tables
- Added unique indexes for Username and Email
- Configured foreign key relationships with cascade delete
- Set up default values for date fields using SQLite functions"
```

### Step 6: Push to GitHub

```bash
# Push all commits to GitHub
git push -u origin main
```

### Step 7: Verification

1. **Check your GitHub repository** - you should see 4 commits
2. **Verify project structure** in GitHub matches the original
3. **Ensure .gitignore** is working (no bin/, obj/, or .db files should be tracked)

---

## Commit Message Guidelines

Each commit message should follow this format:

```
<type>: <user-story> - <brief description>

<detailed description>
- Bullet point of what was added
- Another bullet point
- More details if needed
```

**Types:**
- `feat:` - New feature
- `chore:` - Maintenance task
- `docs:` - Documentation
- `fix:` - Bug fix

**User Stories** should follow format: `US001`, `US002`, etc.

---

## Expected Repository Structure

After completing all commits, your repository should have:

```
StudentDiary-YourName/
├── .gitignore
├── StudentDiary.sln
├── StudentDiary.Infrastructure/
│   ├── Data/
│   │   └── StudentDiaryContext.cs
│   ├── Migrations/
│   │   ├── InitialCreate.cs
│   │   ├── InitialCreate.Designer.cs
│   │   └── StudentDiaryContextModelSnapshot.cs
│   ├── Models/
│   │   ├── User.cs
│   │   └── DiaryEntry.cs
│   └── StudentDiary.Infrastructure.csproj
├── StudentDiary.Services/
│   ├── DTOs/
│   │   ├── UserDTOs.cs
│   │   └── DiaryEntryDTOs.cs
│   ├── Interfaces/
│   │   ├── IAuthService.cs
│   │   └── IDiaryService.cs
│   └── StudentDiary.Services.csproj
└── StudentDiary.Presentation/
    ├── Controllers/
    ├── Views/
    ├── Models/
    ├── wwwroot/
    ├── appsettings.json
    ├── Program.cs
    └── StudentDiary.Presentation.csproj
```

---

## Common Mistakes to Avoid

❌ **Don't do this:**
- Committing all files at once with `git add .`
- Using vague commit messages like "Initial commit" or "Added files"
- Forgetting to push to GitHub
- Including bin/, obj/, or database files in commits

✅ **Do this:**
- Commit each layer separately
- Write descriptive commit messages
- Follow the user story format
- Use proper .gitignore file
- Verify your repository on GitHub

---

## Submission Requirements

1. **GitHub Repository URL** - Submit the link to your repository
2. **Repository must be public** so it can be accessed for grading
3. **Exactly 4 commits** following the specified format
4. **Proper commit messages** using the user story format
5. **Clean repository** with no build artifacts or database files

---

## Grading Criteria (100 points total)

| Criteria | Points | Description |
|----------|--------|-------------|
| **Repository Setup** | 20 | Correct GitHub setup, proper naming, public visibility |
| **Commit Structure** | 30 | Exactly 4 commits, each containing correct project layer |
| **Commit Messages** | 30 | Proper format, user story references, descriptive content |
| **Code Organization** | 15 | Proper project structure, all files in correct locations |
| **Git Best Practices** | 5 | Proper .gitignore, no build artifacts, clean history |

---

## Help and Troubleshooting

### If you make a mistake:
```bash
# To undo the last commit (keep changes)
git reset --soft HEAD~1

# To check commit history
git log --oneline

# To check what files are staged
git status
```

### If you need to start over:
```bash
# Delete .git folder and start fresh
rm -rf .git
git init
# Follow steps again
```

---

## Deadline
**Submit your GitHub repository URL by [INSERT DEADLINE HERE]**

## Questions?
Contact your instructor or post in the class discussion forum.

---

**Remember:** This assignment simulates real-world development practices where features are developed incrementally and committed with meaningful messages. Take your time to understand the project structure and write thoughtful commit messages.