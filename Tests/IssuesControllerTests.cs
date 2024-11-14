using Xunit;
using IssueTracking.Controllers;
using IssueTracking.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using System.Net.Http.Json;
using System.Net.Http;
using System.Net;
using Microsoft.EntityFrameworkCore.InMemory;

namespace IssueTracking.Tests
{
    public class IssuesControllerTests
    {
        private readonly HttpClient _client;
        private readonly ApplicationDbContext _context;

        public IssuesControllerTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new ApplicationDbContext(options);

            var server = new TestServer(new WebHostBuilder()
                .ConfigureServices(services =>
                {
                    services.AddDbContext<ApplicationDbContext>(options =>
                        options.UseInMemoryDatabase("TestDatabase"));
                    services.AddControllers();
                })
                .Configure(app =>
                {
                    app.UseRouting();
                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapControllers();
                    });
                }));

            _client = server.CreateClient();
        }

        [Fact]
        public async Task GetIssues_ReturnsOkResult()
        {
            // Arrange
            var issue = new Issue
            {
                Title = "Test Issue",
                Description = "This is a test issue",
                Status = IssueStatus.Open,
                ProjectId = 1
            };
            _context.Issues.Add(issue);
            _context.SaveChanges();

            // Act
            var response = await _client.GetAsync("/api/issues");

            // Assert
            response.EnsureSuccessStatusCode();
            var issues = await response.Content.ReadFromJsonAsync<List<Issue>>();
            Assert.Single(issues);
        }

        [Fact]
        public async Task GetIssue_ReturnsOkResult()
        {
            // Arrange
            var issue = new Issue
            {
                Title = "Test Issue",
                Description = "This is a test issue",
                Status = IssueStatus.Open,
                ProjectId = 1
            };
            _context.Issues.Add(issue);
            _context.SaveChanges();

            // Act
            var response = await _client.GetAsync($"/api/issues/{issue.Id}");

            // Assert
            response.EnsureSuccessStatusCode();
            var returnedIssue = await response.Content.ReadFromJsonAsync<Issue>();
            Assert.Equal(issue.Id, returnedIssue.Id);
        }

        [Fact]
        public async Task CreateIssue_ReturnsOkResult()
        {
            // Arrange
            var issueModel = new IssueModel
            {
                Title = "Test Issue",
                Description = "This is a test issue",
                Status = IssueStatus.Open,
                ProjectId = 1
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/issues", issueModel);

            // Assert
            response.EnsureSuccessStatusCode();
            var createdIssue = await response.Content.ReadFromJsonAsync<Issue>();
            Assert.Equal(issueModel.Title, createdIssue.Title);
        }

        [Fact]
        public async Task UpdateIssue_ReturnsOkResult()
        {
            // Arrange
            var issue = new Issue
            {
                Title = "Test Issue",
                Description = "This is a test issue",
                Status = IssueStatus.Open,
                ProjectId = 1
            };
            _context.Issues.Add(issue);
            _context.SaveChanges();

            var updatedIssueModel = new IssueModel
            {
                Title = "Updated Test Issue",
                Description = "This is an updated test issue",
                Status = IssueStatus.InProgress,
                ProjectId = 1
            };

            // Act
            var response = await _client.PutAsJsonAsync($"/api/issues/{issue.Id}", updatedIssueModel);

            // Assert
            response.EnsureSuccessStatusCode();
            var updatedIssue = await response.Content.ReadFromJsonAsync<Issue>();
            Assert.Equal(updatedIssueModel.Title, updatedIssue.Title);
        }

        [Fact]
        public async Task DeleteIssue_ReturnsOkResult()
        {
            // Arrange
            var issue = new Issue
            {
                Title = "Test Issue",
                Description = "This is a test issue",
                Status = IssueStatus.Open,
                ProjectId = 1
            };
            _context.Issues.Add(issue);
            _context.SaveChanges();

            // Act
            var response = await _client.DeleteAsync($"/api/issues/{issue.Id}");

            // Assert
            response.EnsureSuccessStatusCode();
            var deletedIssue = await _context.Issues.FindAsync(issue.Id);
            Assert.Null(deletedIssue);
        }
    }
}
