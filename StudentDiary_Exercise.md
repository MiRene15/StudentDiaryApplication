# Student Diary Application - Code-First Migration Exercise

## Overview
Build a Student Diary application using ASP.NET Core MVC with a layered architecture and code-first Entity Framework Core migrations with SQLite.

## Learning Objectives
- Implement code-first database migrations with Entity Framework Core
- Create a layered architecture with proper separation of concerns
- Build user authentication without ASP.NET Core Identity
- Implement CRUD operations with rich text support
- Practice proper version control with meaningful commits

## Project Structure
```
StudentDiary/
├── StudentDiary.Infrastructure/     # Data layer (DbContext and Models)
├── StudentDiary.Services/           # Business logic layer
└── StudentDiary.Presentation/       # MVC Web application
```

## User Stories & Features
Each user story should be implemented as a separate commit:

1. **US001**: As a new user, I want to register an account so I can access the diary
2. **US002**: As a registered user, I want to login to access my diary
3. **US003**: As a user, I want to reset my password if I forget it
4. **US004**: As a user, I want to view and edit my profile information
5. **US005**: As a user, I want account lockout protection after 3 failed login attempts
6. **US006**: As a logged-in user, I want to create diary entries with rich text
7. **US007**: As a user, I want to view all my diary entries
8. **US008**: As a user, I want to edit and delete my diary entries
9. **US009**: As a user, I want to upload a profile picture

## Required Commits Structure
Each commit should represent one completed user story:

- `feat: US001 - Add user registration functionality`
- `feat: US002 - Implement user login system`
- `feat: US003 - Add password reset feature`
- `feat: US004 - Create user profile management`
- `feat: US005 - Implement account lockout mechanism`
- `feat: US006 - Add diary entry creation with rich text`
- `feat: US007 - Implement diary entry listing`
- `feat: US008 - Add diary entry edit/delete functionality`
- `feat: US009 - Add profile picture upload feature`

---

## Part 1: Project Setup

### Step 1: Create Solution and Projects

```bash
# Create solution
dotnet new sln -n StudentDiary

# Create projects
dotnet new classlib -n StudentDiary.Infrastructure
dotnet new classlib -n StudentDiary.Services
dotnet new mvc -n StudentDiary.Presentation

# Add projects to solution
dotnet sln add StudentDiary.Infrastructure/StudentDiary.Infrastructure.csproj
dotnet sln add StudentDiary.Services/StudentDiary.Services.csproj
dotnet sln add StudentDiary.Presentation/StudentDiary.Presentation.csproj
```

### Step 2: Add Project References

```bash
# Services references Infrastructure
dotnet add StudentDiary.Services/StudentDiary.Services.csproj reference StudentDiary.Infrastructure/StudentDiary.Infrastructure.csproj

# Presentation references Services
dotnet add StudentDiary.Presentation/StudentDiary.Presentation.csproj reference StudentDiary.Services/StudentDiary.Services.csproj
```

### Step 3: Install NuGet Packages

**StudentDiary.Infrastructure:**
```bash
dotnet add StudentDiary.Infrastructure package Microsoft.EntityFrameworkCore
dotnet add StudentDiary.Infrastructure package Microsoft.EntityFrameworkCore.Sqlite
dotnet add StudentDiary.Infrastructure package Microsoft.EntityFrameworkCore.Tools
```

**StudentDiary.Presentation:**
```bash
dotnet add StudentDiary.Presentation package Microsoft.EntityFrameworkCore.Design
```

---

## Part 2: Infrastructure Layer

### Step 4: Create Entity Models

Create `StudentDiary.Infrastructure/Models/User.cs`:
```csharp
using System.ComponentModel.DataAnnotations;

namespace StudentDiary.Infrastructure.Models
{
    public class User
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Username { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Email { get; set; }
        
        [Required]
        public string PasswordHash { get; set; }
        
        [StringLength(50)]
        public string FirstName { get; set; }
        
        [StringLength(50)]
        public string LastName { get; set; }
        
        public string ProfilePicturePath { get; set; }
        
        public DateTime DateCreated { get; set; }
        
        public DateTime LastLoginDate { get; set; }
        
        public int FailedLoginAttempts { get; set; }
        
        public DateTime? LockoutEnd { get; set; }
        
        public string PasswordResetToken { get; set; }
        
        public DateTime? PasswordResetTokenExpiry { get; set; }
        
        // Navigation property
        public virtual ICollection<DiaryEntry> DiaryEntries { get; set; } = new List<DiaryEntry>();
    }
}
```

Create `StudentDiary.Infrastructure/Models/DiaryEntry.cs`:
```csharp
using System.ComponentModel.DataAnnotations;

namespace StudentDiary.Infrastructure.Models
{
    public class DiaryEntry
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Title { get; set; }
        
        [Required]
        public string Content { get; set; } // Rich text content
        
        public DateTime CreatedDate { get; set; }
        
        public DateTime LastModifiedDate { get; set; }
        
        // Foreign key
        public int UserId { get; set; }
        
        // Navigation property
        public virtual User User { get; set; }
    }
}
```

### Step 5: Create DbContext

Create `StudentDiary.Infrastructure/Data/StudentDiaryContext.cs`:
```csharp
using Microsoft.EntityFrameworkCore;
using StudentDiary.Infrastructure.Models;

namespace StudentDiary.Infrastructure.Data
{
    public class StudentDiaryContext : DbContext
    {
        public StudentDiaryContext(DbContextOptions<StudentDiaryContext> options) : base(options)
        {
        }
        
        public DbSet<User> Users { get; set; }
        public DbSet<DiaryEntry> DiaryEntries { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // User configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.DateCreated).HasDefaultValueSql("datetime('now')");
            });
            
            // DiaryEntry configuration
            modelBuilder.Entity<DiaryEntry>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("datetime('now')");
                entity.Property(e => e.LastModifiedDate).HasDefaultValueSql("datetime('now')");
                
                // Configure relationship
                entity.HasOne(d => d.User)
                      .WithMany(u => u.DiaryEntries)
                      .HasForeignKey(d => d.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
```

### Step 6: Complete Infrastructure Setup

The Infrastructure layer is now complete with:
- Entity Models (User and DiaryEntry)
- DbContext (StudentDiaryContext)
- The Services layer will use the DbContext directly for data operations

---

## Part 3: Services Layer

### Step 8: Create DTOs

Create `StudentDiary.Services/DTOs/UserDTOs.cs`:
```csharp
using System.ComponentModel.DataAnnotations;

namespace StudentDiary.Services.DTOs
{
    public class RegisterDto
    {
        [Required]
        [StringLength(50)]
        public string Username { get; set; }
        
        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }
        
        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; }
        
        [Required]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
        
        [StringLength(50)]
        public string FirstName { get; set; }
        
        [StringLength(50)]
        public string LastName { get; set; }
    }
    
    public class LoginDto
    {
        [Required]
        public string Username { get; set; }
        
        [Required]
        public string Password { get; set; }
        
        public bool RememberMe { get; set; }
    }
    
    public class ForgotPasswordDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
    
    public class ResetPasswordDto
    {
        [Required]
        public string Token { get; set; }
        
        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string NewPassword { get; set; }
        
        [Required]
        [Compare("NewPassword")]
        public string ConfirmPassword { get; set; }
    }
    
    public class UserProfileDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProfilePicturePath { get; set; }
        public DateTime DateCreated { get; set; }
    }
    
    public class UpdateProfileDto
    {
        [StringLength(50)]
        public string FirstName { get; set; }
        
        [StringLength(50)]
        public string LastName { get; set; }
        
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }
    }
}
```

Create `StudentDiary.Services/DTOs/DiaryEntryDTOs.cs`:
```csharp
using System.ComponentModel.DataAnnotations;

namespace StudentDiary.Services.DTOs
{
    public class CreateDiaryEntryDto
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; }
        
        [Required]
        public string Content { get; set; }
    }
    
    public class UpdateDiaryEntryDto
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Title { get; set; }
        
        [Required]
        public string Content { get; set; }
    }
    
    public class DiaryEntryDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public int UserId { get; set; }
    }
}
```

### Step 9: Create Service Interfaces

Create `StudentDiary.Services/Interfaces/IAuthService.cs`:
```csharp
using StudentDiary.Services.DTOs;

namespace StudentDiary.Services.Interfaces
{
    public interface IAuthService
    {
        Task<(bool Success, string Message)> RegisterAsync(RegisterDto registerDto);
        Task<(bool Success, string Message, UserProfileDto User)> LoginAsync(LoginDto loginDto);
        Task<(bool Success, string Message)> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto);
        Task<(bool Success, string Message)> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
        Task<UserProfileDto> GetUserProfileAsync(int userId);
        Task<(bool Success, string Message)> UpdateProfileAsync(int userId, UpdateProfileDto updateProfileDto);
        Task<(bool Success, string Message)> UpdateProfilePictureAsync(int userId, string imagePath);
        string HashPassword(string password);
        bool VerifyPassword(string password, string hash);
    }
}
```

Create `StudentDiary.Services/Interfaces/IDiaryService.cs`:
```csharp
using StudentDiary.Services.DTOs;

namespace StudentDiary.Services.Interfaces
{
    public interface IDiaryService
    {
        Task<IEnumerable<DiaryEntryDto>> GetUserEntriesAsync(int userId);
        Task<DiaryEntryDto> GetEntryByIdAsync(int entryId, int userId);
        Task<(bool Success, string Message, DiaryEntryDto Entry)> CreateEntryAsync(int userId, CreateDiaryEntryDto createDto);
        Task<(bool Success, string Message, DiaryEntryDto Entry)> UpdateEntryAsync(int userId, UpdateDiaryEntryDto updateDto);
        Task<(bool Success, string Message)> DeleteEntryAsync(int entryId, int userId);
    }
}
```

### Step 10: Example Service Implementation Structure

The Services will use the DbContext directly. Here's an example structure for `AuthService`:

```csharp
using Microsoft.EntityFrameworkCore;
using StudentDiary.Infrastructure.Data;
using StudentDiary.Infrastructure.Models;
using StudentDiary.Services.DTOs;
using StudentDiary.Services.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace StudentDiary.Services.Services
{
    public class AuthService : IAuthService
    {
        private readonly StudentDiaryContext _context;
        
        public AuthService(StudentDiaryContext context)
        {
            _context = context;
        }
        
        // Example method - students will implement the full logic
        public async Task<(bool Success, string Message)> RegisterAsync(RegisterDto registerDto)
        {
            // Check if user already exists
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == registerDto.Username || u.Email == registerDto.Email);
                
            if (existingUser != null)
            {
                return (false, "Username or email already exists");
            }
            
            // Create new user
            var user = new User
            {
                Username = registerDto.Username,
                Email = registerDto.Email,
                PasswordHash = HashPassword(registerDto.Password),
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                DateCreated = DateTime.UtcNow
            };
            
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            
            return (true, "User registered successfully");
        }
        
        // Students will implement other methods...
        public string HashPassword(string password)
        {
            // Simple hash implementation - students can improve this
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }
        
        // Other interface methods to be implemented by students...
    }
}
```

---

## Part 4: Create Migration and Database

### Step 11: Configure Connection String

In `StudentDiary.Presentation/appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=StudentDiary.db"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

### Step 12: Configure Services in Program.cs

Update `StudentDiary.Presentation/Program.cs`:
```csharp
using Microsoft.EntityFrameworkCore;
using StudentDiary.Infrastructure.Data;
using StudentDiary.Services.Interfaces;
// using StudentDiary.Services.Services; // Students will add this when implementing services

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure Entity Framework
builder.Services.AddDbContext<StudentDiaryContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register services - Students will uncomment these after implementing the services
// builder.Services.AddScoped<IAuthService, AuthService>();
// builder.Services.AddScoped<IDiaryService, DiaryService>();

// Configure session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
```

### Step 13: Create and Run Migration

Navigate to the Presentation project directory and run:

```bash
# Navigate to presentation project
cd StudentDiary.Presentation

# Add initial migration
dotnet ef migrations add InitialCreate --project ../StudentDiary.Infrastructure --context StudentDiaryContext

# Update database
dotnet ef database update --project ../StudentDiary.Infrastructure --context StudentDiaryContext
```

---

## Part 5: Implementation Tasks

### Task 1: Complete the Service Implementations
Students should implement:
1. `AuthService` class implementing `IAuthService` - uses DbContext directly for user operations
2. `DiaryService` class implementing `IDiaryService` - uses DbContext directly for diary entry operations

**Key Points:**
- Services should inject `StudentDiaryContext` in their constructors
- Use Entity Framework methods directly (e.g., `_context.Users.FirstOrDefaultAsync()`)
- Handle business logic validation in the services
- Return appropriate success/failure messages

### Task 2: Create Controllers
Students should create:
1. `AuthController` - Handle registration, login, logout, password reset
2. `DiaryController` - Handle CRUD operations for diary entries
3. `ProfileController` - Handle user profile management

### Task 3: Create Views
Students should create appropriate Razor views for:
1. Registration form
2. Login form
3. Password reset forms
4. User profile pages
5. Diary entry list, create, edit views
6. Rich text editor integration (using a library like TinyMCE or CKEditor)

### Task 4: Add Authentication Middleware
Implement custom authentication using sessions (not Identity).

### Task 5: Add File Upload for Profile Pictures
Implement profile picture upload functionality with proper validation.

---

## Requirements Summary

### Authentication Features (No Identity)
- ✅ User registration with validation
- ✅ Login with username/password
- ✅ Password hashing and verification
- ✅ Password reset via email token
- ✅ Account lockout after 3 failed attempts
- ✅ Session-based authentication

### Diary Features
- ✅ Rich text diary entries
- ✅ CRUD operations for entries
- ✅ Title and date for each entry
- ✅ User-specific entries (authorization)

### Profile Features
- ✅ User profile management
- ✅ Profile picture upload
- ✅ User information editing

### Security Features
- ✅ Access control (no diary access without login)
- ✅ Password hashing
- ✅ Account lockout mechanism
- ✅ Secure password reset tokens

---

## Bonus Challenges
1. Add diary entry search functionality
2. Implement diary entry categories/tags
3. Add export functionality (PDF/Word)
4. Implement diary entry sharing
5. Add email notifications for password reset
6. Implement data encryption for diary content

## Grading Criteria
- **Code Quality (30%)**: Clean, readable, well-structured code
- **Functionality (40%)**: All required features working correctly
- **Database Design (15%)**: Proper entity relationships and migrations
- **Security (10%)**: Proper password handling and access control
- **Version Control (5%)**: Proper commit structure following user stories

Remember: Each user story should be a separate commit with a meaningful message!