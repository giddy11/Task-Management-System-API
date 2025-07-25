using TaskManagement.Domain;
using TaskManagement.Domain.Projects;
using TaskManagement.Domain.TodoTasks;
using TaskManagement.Domain.UserManagement;
using TaskManagement.Persistence;

namespace TaskManagement.API;

public static class DatabaseSeeder
{
    public static void Seed(TaskManagementDbContext context)
    {
        if (context.Users.Any() ||
            context.Projects.Any() ||
            context.Labels.Any() ||
            context.Tasks.Any() ||
            context.Comments.Any())
        {
            return;
        }

        var now = DateTime.UtcNow;
        var users = CreateUsers(now);
        var projects = CreateProjects(users, now);
        var labels = CreateLabels(users, now);
        var tasks = CreateTasks(users, projects, labels, now);
        var comments = CreateComments(users, tasks, now);

        context.Users.AddRange(users);
        context.Projects.AddRange(projects);
        context.Labels.AddRange(labels);
        context.Tasks.AddRange(tasks);
        context.Comments.AddRange(comments);

        context.SaveChanges();
    }

    private static List<User> CreateUsers(DateTime now)
    {
        return
        [
            new()
            {
                Id = Guid.NewGuid(),
                Email = "admin@taskmanager.com",
                FirstName = "Admin",
                LastName = "User",
                AccountType = AccountTypes.Admin,
            },
            new()
            {
                Email = "john.doe@example.com",
                FirstName = "John",
                LastName = "Doe",
            },
            new()
            {
                Email = "jane.smith@example.com",
                FirstName = "Jane",
                LastName = "Smith",
            },
            new()
            {
                Email = "mike.johnson@example.com",
                FirstName = "Mike",
                LastName = "Johnson",
            },
            new()
            {
                Email = "sarah.williams@example.com",
                FirstName = "Sarah",
                LastName = "Williams",
            },
            new() 
            {
                Email = "david.brown@example.com",
                FirstName = "David",
                LastName = "Brown",
            }
        ];
    }

    private static List<Project> CreateProjects(List<User> users, DateTime now)
    {
        var admin = users.First(u => u.AccountType == AccountTypes.Admin);

        return
        [
            new() {
                Id = Guid.NewGuid(),
                Title = "Website Redesign",
                Description = "Complete redesign of company website with modern UI/UX",
                CreatedById = admin.Id,
                StartDate = now.AddDays(-30),
                EndDate = now.AddDays(60),
                ProjectStatus = ProjectStatus.In_Progress,
            },
            new() {
                Title = "Mobile App Development",
                Description = "Build cross-platform mobile application for iOS and Android",
                CreatedById = admin.Id,
                StartDate = now.AddDays(-15),
                EndDate = now.AddDays(90),
                ProjectStatus = ProjectStatus.Not_Started,
            },
            new() {
                Title = "Marketing Campaign",
                Description = "Q3 marketing campaign preparation and execution",
                CreatedById = admin.Id,
                StartDate = now.AddDays(-7),
                EndDate = now.AddDays(45),
                ProjectStatus = ProjectStatus.Not_Started,
            },
            new() {
                Title = "Internal Tools Upgrade",
                Description = null,
                CreatedById = admin.Id,
                StartDate = now,
                EndDate = now.AddDays(30),
                ProjectStatus = ProjectStatus.Not_Started,
            },
            new() {
                Title = "Customer Support Portal",
                Description = "New portal for customer support tickets",
                CreatedById = admin.Id,
                StartDate = now.AddDays(-10),
                EndDate = now.AddDays(120),
                ProjectStatus = ProjectStatus.Not_Started,
            }
        ];
    }

    private static List<Label> CreateLabels(List<User> users, DateTime now)
    {
        var admin = users.First(u => u.AccountType == AccountTypes.Admin);

        return
        [
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Frontend",
                Color = "#FF5733",
                CreatedById = admin.Id,
            },
            new() 
            {
                Name = "Backend",
                Color = "#33FF57",
                CreatedById = admin.Id,
            },
            new() 
            {
                Name = "UI/UX",
                Color = "#3357FF",
                CreatedById = admin.Id,
            },
            new() 
            {
                Name = "Bug",
                Color = "#FF3333",
                CreatedById = admin.Id,
            },
            new() 
            {
                Name = "Feature",
                Color = "#33FFF5",
                CreatedById = admin.Id,
            },
            new() 
            {
                Name = "Documentation",
                Color = "#F5FF33",
                CreatedById = admin.Id,
            },
            new() 
            {
                Name = "High Priority",
                Color = "#FF33F5",
                CreatedById = admin.Id,
            }
        ];
    }

    private static List<TodoTask> CreateTasks(List<User> users, List<Project> projects, List<Label> labels, DateTime now)
    {
        var admin = users.First(u => u.AccountType == AccountTypes.Admin);
        var john = users.First(u => u.FirstName == "John");
        var jane = users.First(u => u.FirstName == "Jane");
        var mike = users.First(u => u.FirstName == "Mike");
        var sarah = users.First(u => u.FirstName == "Sarah");

        var websiteProject = projects.First(p => p.Title == "Website Redesign");
        var mobileProject = projects.First(p => p.Title == "Mobile App Development");
        var marketingProject = projects.First(p => p.Title == "Marketing Campaign");

        var frontendLabel = labels.First(l => l.Name == "Frontend");
        var backendLabel = labels.First(l => l.Name == "Backend");
        var uiuxLabel = labels.First(l => l.Name == "UI/UX");
        var bugLabel = labels.First(l => l.Name == "Bug");
        var featureLabel = labels.First(l => l.Name == "Feature");
        var highPriorityLabel = labels.First(l => l.Name == "High Priority");

        var tasks = new List<TodoTask>
        {
            new() 
            {
                Title = "Design homepage layout",
                CreatedById = admin.Id,
                ProjectId = websiteProject.Id,
                Description = "Create wireframes and final design for the homepage",
                StartDate = now.AddDays(-25),
                EndDate = now.AddDays(5),
                TodoTaskStatus = TodoTaskStatus.In_Progress,
                PriorityStatus = PriorityStatus.Medium,
                Assignees = [john, jane],
                Labels = [uiuxLabel, frontendLabel]
            },
            new() {
                Title = "Implement user authentication",
                CreatedById = admin.Id,
                ProjectId = websiteProject.Id,
                Description = "Setup JWT authentication for the API",
                StartDate = now.AddDays(-20),
                EndDate = now.AddDays(10),
                TodoTaskStatus = TodoTaskStatus.In_Progress,
                PriorityStatus = PriorityStatus.High,
                Assignees = [mike],
                Labels = [backendLabel, highPriorityLabel]
            },
            new() {
                Title = "Fix mobile menu bug",
                CreatedById = admin.Id,
                ProjectId = websiteProject.Id,
                Description = "Menu doesn't close properly on mobile devices",
                StartDate = now.AddDays(-5),
                EndDate = now.AddDays(2),
                TodoTaskStatus = TodoTaskStatus.Todo,
                PriorityStatus = PriorityStatus.High,
                Assignees = [john],
                Labels = [frontendLabel, bugLabel, highPriorityLabel]
            },
            new() {
                Title = "Create app splash screen",
                CreatedById = admin.Id,
                ProjectId = mobileProject.Id,
                Description = "Design and implement splash screen animation",
                StartDate = now.AddDays(-10),
                EndDate = now.AddDays(15),
                TodoTaskStatus = TodoTaskStatus.In_Progress,
                PriorityStatus = PriorityStatus.Low,
                Assignees = [jane],
                Labels = [uiuxLabel, frontendLabel]
            },
            new() {
                Title = "Setup push notifications",
                CreatedById = admin.Id,
                ProjectId = mobileProject.Id,
                Description = "Implement Firebase push notifications",
                StartDate = now.AddDays(-5),
                EndDate = now.AddDays(30),
                TodoTaskStatus = TodoTaskStatus.Todo,
                PriorityStatus = PriorityStatus.Medium,
                Assignees = [mike],
                Labels = [backendLabel, featureLabel]
            },
            new() {
                Title = "Design campaign banners",
                CreatedById = admin.Id,
                ProjectId = marketingProject.Id,
                Description = "Create banners for social media and website",
                StartDate = now.AddDays(-3),
                EndDate = now.AddDays(7),
                TodoTaskStatus = TodoTaskStatus.Todo,
                PriorityStatus = PriorityStatus.High,
                Assignees = [sarah],
                Labels = [uiuxLabel]
            }
        };

        return tasks;
    }

    private static List<Comment> CreateComments(List<User> users, List<TodoTask> tasks, DateTime now)
    {
        var john = users.First(u => u.FirstName == "John");
        var jane = users.First(u => u.FirstName == "Jane");
        var mike = users.First(u => u.FirstName == "Mike");

        var homepageTask = tasks.First(t => t.Title == "Design homepage layout");
        var authTask = tasks.First(t => t.Title == "Implement user authentication");
        var menuBugTask = tasks.First(t => t.Title == "Fix mobile menu bug");

        return
        [
            new() 
            {
                UserId = john.Id,
                TodoTaskId = homepageTask.Id,
                Content = "I've completed the initial wireframes. Please review when you have time.",
                CreatedAt = now,
                UpdatedAt = now
            },
            new() 
            {
                UserId = jane.Id,
                TodoTaskId = homepageTask.Id,
                Content = "The wireframes look good, but we should consider adding a featured products section.",
                CreatedAt = now,
                UpdatedAt = now
            },
            new() 
            {
                UserId = john.Id,
                TodoTaskId = homepageTask.Id,
                Content = "Added the featured products section. Updated designs are in Figma.",
                CreatedAt = now,
                UpdatedAt = now
            },
            new() 
            {
                UserId = mike.Id,
                TodoTaskId = authTask.Id,
                Content = "Authentication API is complete. Need frontend team to integrate.",
                CreatedAt = now,
                UpdatedAt = now
            },
            new() 
            {
                UserId = john.Id,
                TodoTaskId = menuBugTask.Id,
                Content = "I've reproduced the issue. It seems to be related to the click event handler.",
                CreatedAt = now,
                UpdatedAt = now
            },
            new() 
            {
                UserId = jane.Id,
                TodoTaskId = menuBugTask.Id,
                Content = "I'll take a look at this tomorrow morning.",
                CreatedAt = now,
                UpdatedAt = now
            }
        ];
    }
}