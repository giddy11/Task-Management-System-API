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
        if (!context.Users.Any())
        {
            var users = CreateUsers();
            context.Users.AddRange(users);
            context.SaveChanges();

            var projects = CreateProjects(users);
            context.Projects.AddRange(projects);
            context.SaveChanges();

            var labels = CreateLabels(users);
            context.Labels.AddRange(labels);
            context.SaveChanges();

            var tasks = CreateTasks(users, projects, labels);
            context.Tasks.AddRange(tasks);
            context.SaveChanges();

            var comments = CreateComments(users, tasks);
            context.Comments.AddRange(comments);
            context.SaveChanges();
        }
    }

    private static List<User> CreateUsers()
    {
        return new List<User>
        {
            User.New("admin@taskmanager.com", "Admin", "User", AccountTypes.Admin, Guid.Parse("11111111-1111-1111-1111-111111111111")),
            User.New("john.doe@example.com", "John", "Doe"),
            User.New("jane.smith@example.com", "Jane", "Smith"),
            User.New("mike.johnson@example.com", "Mike", "Johnson"),
            User.New("sarah.williams@example.com", "Sarah", "Williams"),
            User.New("david.brown@example.com", "David", "Brown")
        };
    }

    private static List<Project> CreateProjects(List<User> users)
    {
        var admin = users.First(u => u.AccountType == AccountTypes.Admin);
        var now = DateTime.UtcNow;

        return new List<Project>
        {
            Project.New(
                "Website Redesign",
                "Complete redesign of company website with modern UI/UX",
                admin.Id,
                now.AddDays(-30),
                now.AddDays(60),
                ProjectStatus.In_Progress,
                Guid.Parse("22222222-2222-2222-2222-222222222222")),

            Project.New(
                "Mobile App Development",
                "Build cross-platform mobile application for iOS and Android",
                admin.Id,
                now.AddDays(-15),
                now.AddDays(90)),

            Project.New(
                "Marketing Campaign",
                "Q3 marketing campaign preparation and execution",
                admin.Id,
                now.AddDays(-7),
                now.AddDays(45)),

            Project.New(
                "Internal Tools Upgrade",
                null,
                admin.Id,
                now,
                now.AddDays(30)),

            Project.New(
                "Customer Support Portal",
                "New portal for customer support tickets",
                admin.Id,
                now.AddDays(-10),
                now.AddDays(120),
                ProjectStatus.Not_Started)
        };
    }

    private static List<Label> CreateLabels(List<User> users)
    {
        var admin = users.First(u => u.AccountType == AccountTypes.Admin);

        return new List<Label>
        {
            Label.New("Frontend", "#FF5733", admin.Id, Guid.Parse("33333333-3333-3333-3333-333333333333")),
            Label.New("Backend", "#33FF57", admin.Id),
            Label.New("UI/UX", "#3357FF", admin.Id),
            Label.New("Bug", "#FF3333", admin.Id),
            Label.New("Feature", "#33FFF5", admin.Id),
            Label.New("Documentation", "#F5FF33", admin.Id),
            Label.New("High Priority", "#FF33F5", admin.Id)
        };
    }

    private static List<TodoTask> CreateTasks(List<User> users, List<Project> projects, List<Label> labels)
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

        var now = DateTime.UtcNow;

        var tasks = new List<TodoTask>
        {
            TodoTask.New(
                "Design homepage layout",
                admin.Id,
                websiteProject.Id,
                "Create wireframes and final design for the homepage",
                now.AddDays(-25),
                now.AddDays(5),
                TodoTaskStatus.In_Progress,
                PriorityStatus.Medium)
            .AssignToUser(john)
            .AssignToUser(jane)
            .ChangeStatus(TodoTaskStatus.In_Progress),

            TodoTask.New(
                "Implement user authentication",
                admin.Id,
                websiteProject.Id,
                "Setup JWT authentication for the API",
                now.AddDays(-20),
                now.AddDays(10),
                TodoTaskStatus.In_Progress,
                PriorityStatus.High)
            .AssignToUser(mike)
            .ChangeStatus(TodoTaskStatus.In_Progress),

            TodoTask.New(
                "Fix mobile menu bug",
                admin.Id,
                websiteProject.Id,
                "Menu doesn't close properly on mobile devices",
                now.AddDays(-5),
                now.AddDays(2),
                TodoTaskStatus.Todo,
                PriorityStatus.High)
            .AssignToUser(john)
            .ChangeStatus(TodoTaskStatus.Todo),

            TodoTask.New(
                "Create app splash screen",
                admin.Id,
                mobileProject.Id,
                "Design and implement splash screen animation",
                now.AddDays(-10),
                now.AddDays(15),
                TodoTaskStatus.In_Progress,
                PriorityStatus.Low)
            .AssignToUser(jane),

            TodoTask.New(
                "Setup push notifications",
                admin.Id,
                mobileProject.Id,
                "Implement Firebase push notifications",
                now.AddDays(-5),
                now.AddDays(30),
                TodoTaskStatus.Todo,
                PriorityStatus.Medium)
            .AssignToUser(mike),

            TodoTask.New(
                "Design campaign banners",
                admin.Id,
                marketingProject.Id,
                "Create banners for social media and website",
                now.AddDays(-3),
                now.AddDays(7),
                TodoTaskStatus.Todo,
                PriorityStatus.High)
            .AssignToUser(sarah)
        };

        // Add labels to tasks
        tasks[0].Labels.Add(uiuxLabel);
        tasks[0].Labels.Add(frontendLabel);
        tasks[1].Labels.Add(backendLabel);
        tasks[1].Labels.Add(highPriorityLabel);
        tasks[2].Labels.Add(frontendLabel);
        tasks[2].Labels.Add(bugLabel);
        tasks[2].Labels.Add(highPriorityLabel);
        tasks[3].Labels.Add(uiuxLabel);
        tasks[3].Labels.Add(frontendLabel);
        tasks[4].Labels.Add(backendLabel);
        tasks[4].Labels.Add(featureLabel);
        tasks[5].Labels.Add(uiuxLabel);

        return tasks;
    }

    private static List<Comment> CreateComments(List<User> users, List<TodoTask> tasks)
    {
        var john = users.First(u => u.FirstName == "John");
        var jane = users.First(u => u.FirstName == "Jane");
        var mike = users.First(u => u.FirstName == "Mike");

        var homepageTask = tasks.First(t => t.Title == "Design homepage layout");
        var authTask = tasks.First(t => t.Title == "Implement user authentication");
        var menuBugTask = tasks.First(t => t.Title == "Fix mobile menu bug");

        var now = DateTime.UtcNow;

        return new List<Comment>
        {
            Comment.New(
                john.Id,
                homepageTask.Id,
                "I've completed the initial wireframes. Please review when you have time."),

            Comment.New(
                jane.Id,
                homepageTask.Id,
                "The wireframes look good, but we should consider adding a featured products section."),

            Comment.New(
                john.Id,
                homepageTask.Id,
                "Added the featured products section. Updated designs are in Figma."),

            Comment.New(
                mike.Id,
                authTask.Id,
                "Authentication API is complete. Need frontend team to integrate."),

            Comment.New(
                john.Id,
                menuBugTask.Id,
                "I've reproduced the issue. It seems to be related to the click event handler."),

            Comment.New(
                jane.Id,
                menuBugTask.Id,
                "I'll take a look at this tomorrow morning.")
        };
    }
}
