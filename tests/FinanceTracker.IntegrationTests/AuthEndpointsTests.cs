using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FinanceTracker.API.DTOs;
using FluentAssertions;

namespace FinanceTracker.IntegrationTests;

public class AuthEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public AuthEndpointsTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Register_WithValidData_ReturnsOkWithToken()
    {
        var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/auth/register", new
        {
            Email = "newuser@example.com",
            Password = "StrongPass123!",
            FirstName = "Test",
            LastName = "User"
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
        result.Should().NotBeNull();
        result!.Token.Should().NotBeNullOrEmpty();
        result.Email.Should().Be("newuser@example.com");
    }

    [Fact]
    public async Task Register_DuplicateEmail_ReturnsBadRequest()
    {
        var client = _factory.CreateClient();
        var uniqueEmail = $"dup-{Guid.NewGuid():N}@example.com";

        await client.PostAsJsonAsync("/api/auth/register", new
        {
            Email = uniqueEmail,
            Password = "StrongPass123!",
            FirstName = "First",
            LastName = "User"
        });

        var response = await client.PostAsJsonAsync("/api/auth/register", new
        {
            Email = uniqueEmail,
            Password = "AnotherPass1!",
            FirstName = "Second",
            LastName = "User"
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsToken()
    {
        var client = _factory.CreateClient();
        var uniqueEmail = $"login-{Guid.NewGuid():N}@example.com";

        await client.PostAsJsonAsync("/api/auth/register", new
        {
            Email = uniqueEmail,
            Password = "StrongPass123!",
            FirstName = "Test",
            LastName = "User"
        });

        var response = await client.PostAsJsonAsync("/api/auth/login", new
        {
            Email = uniqueEmail,
            Password = "StrongPass123!"
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
        result!.Token.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Login_WithWrongPassword_ReturnsUnauthorized()
    {
        var client = _factory.CreateClient();
        var uniqueEmail = $"wrong-{Guid.NewGuid():N}@example.com";

        await client.PostAsJsonAsync("/api/auth/register", new
        {
            Email = uniqueEmail,
            Password = "StrongPass123!",
            FirstName = "Test",
            LastName = "User"
        });

        var response = await client.PostAsJsonAsync("/api/auth/login", new
        {
            Email = uniqueEmail,
            Password = "BadPassword!"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ProtectedEndpoint_WithoutToken_ReturnsUnauthorized()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/accounts");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ProtectedEndpoint_WithValidToken_ReturnsOk()
    {
        var client = _factory.CreateClient();
        var uniqueEmail = $"authed-{Guid.NewGuid():N}@example.com";

        var registerResponse = await client.PostAsJsonAsync("/api/auth/register", new
        {
            Email = uniqueEmail,
            Password = "StrongPass123!",
            FirstName = "Auth",
            LastName = "User"
        });
        var auth = await registerResponse.Content.ReadFromJsonAsync<AuthResponseDto>();

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", auth!.Token);

        var response = await client.GetAsync("/api/accounts");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
