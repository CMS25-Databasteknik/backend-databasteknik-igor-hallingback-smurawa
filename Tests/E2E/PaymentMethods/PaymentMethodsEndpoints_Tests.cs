using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Backend.Application.Modules.PaymentMethods.Outputs;
using Backend.Infrastructure.Persistence.EFC.Context;
using Backend.Presentation.API.Models;
using Backend.Presentation.API.Models.PaymentMethod;
using Backend.Tests.Integration.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Backend.Tests.E2E.PaymentMethods;

public sealed class PaymentMethodsEndpoints_Tests(CoursesOnlineDbApiFactory factory) : IClassFixture<CoursesOnlineDbApiFactory>
{
    private readonly CoursesOnlineDbApiFactory _factory = factory;
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    [Fact]
    public async Task GetPaymentMethods_ReturnsOk_WithSeededList_AfterReset()
    {
        await _factory.ResetAndSeedDataAsync();
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/payment-methods");
        var payload = await response.Content.ReadFromJsonAsync<PaymentMethodListResult>(_jsonOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(payload);
        Assert.True(payload.Success);
        Assert.NotNull(payload.Result);
        Assert.True(payload.Result.Count >= 3);
    }

    [Fact]
    public async Task GetPaymentMethods_ReturnsNewestFirst_ByIdDescending()
    {
        await _factory.ResetAndSeedDataAsync();
        using var client = _factory.CreateClient();

        var firstCreate = await client.PostAsJsonAsync("/api/payment-methods", new CreatePaymentMethodRequest
        {
            Name = $"OrderA-{Guid.NewGuid():N}"
        });
        var firstId = int.Parse(firstCreate.Headers.Location!.OriginalString.Split('/')[^1]);

        var secondCreate = await client.PostAsJsonAsync("/api/payment-methods", new CreatePaymentMethodRequest
        {
            Name = $"OrderB-{Guid.NewGuid():N}"
        });
        var secondId = int.Parse(secondCreate.Headers.Location!.OriginalString.Split('/')[^1]);

        var response = await client.GetAsync("/api/payment-methods");
        var payload = await response.Content.ReadFromJsonAsync<PaymentMethodListResult>(_jsonOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(payload?.Result);
        Assert.Equal(secondId, payload.Result[0].Id);
        Assert.Equal(firstId, payload.Result[1].Id);
    }

    [Fact]
    public async Task CreatePaymentMethod_ThenGetById_ReturnsCreatedPaymentMethod()
    {
        await _factory.ResetAndSeedDataAsync();
        using var client = _factory.CreateClient();

        var createRequest = new CreatePaymentMethodRequest
        {
            Name = $"Swish-{Guid.NewGuid():N}"
        };

        var createResponse = await client.PostAsJsonAsync("/api/payment-methods", createRequest);

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        Assert.NotNull(createResponse.Headers.Location);

        var createdId = int.Parse(createResponse.Headers.Location!.OriginalString.Split('/')[^1]);
        var getResponse = await client.GetAsync($"/api/payment-methods/{createdId}");
        var getPayload = await getResponse.Content.ReadFromJsonAsync<PaymentMethodResult>(_jsonOptions);

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        Assert.NotNull(getPayload);
        Assert.True(getPayload.Success);
        Assert.NotNull(getPayload.Result);
        Assert.Equal(createdId, getPayload.Result.Id);
        Assert.Equal(createRequest.Name, getPayload.Result.Name);
    }

    [Fact]
    public async Task CreatePaymentMethod_ReturnsBadRequest_ForInvalidPayload()
    {
        await _factory.ResetAndSeedDataAsync();
        using var client = _factory.CreateClient();

        using var content = new StringContent("{\"name\":123}", Encoding.UTF8, "application/json");
        var response = await client.PostAsync("/api/payment-methods", content);
        var payload = await response.Content.ReadFromJsonAsync<ApiResponse>(_jsonOptions);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(payload);
        Assert.False(payload.Success);
        Assert.Equal("validation_error", payload.Code);
    }

    [Fact]
    public async Task GetPaymentMethodById_ReturnsBadRequest_ForInvalidId()
    {
        await _factory.ResetAndSeedDataAsync();
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/payment-methods/-1");
        var payload = await response.Content.ReadFromJsonAsync<ApiResponse>(_jsonOptions);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(payload);
        Assert.False(payload.Success);
        Assert.Equal("validation_error", payload.Code);
    }

    [Fact]
    public async Task DeletePaymentMethod_ReturnsConflict_WhenPaymentMethodIsInUse()
    {
        await _factory.ResetAndSeedDataAsync();

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<CoursesOnlineDbContext>();
            var participant = await RepositoryTestDataHelper.CreateParticipantAsync(db);
            var courseEvent = await RepositoryTestDataHelper.CreateCourseEventAsync(db, seats: 10);
            _ = await RepositoryTestDataHelper.CreateCourseRegistrationAsync(
                db,
                participantId: participant.Id,
                courseEventId: courseEvent.Id);
        }

        using var client = _factory.CreateClient();
        var response = await client.DeleteAsync("/api/payment-methods/1");
        var payload = await response.Content.ReadFromJsonAsync<ApiResponse<bool>>(_jsonOptions);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.NotNull(payload);
        Assert.False(payload.Success);
        Assert.Equal("conflict", payload.Code);
        Assert.False(payload.Result);
    }

    [Fact]
    public async Task DeletePaymentMethod_ReturnsOk_AndRemovesPaymentMethod()
    {
        await _factory.ResetAndSeedDataAsync();

        int paymentMethodId;
        using (var client = _factory.CreateClient())
        {
            var createRequest = new CreatePaymentMethodRequest
            {
                Name = $"TempPay-{Guid.NewGuid():N}"
            };

            var createResponse = await client.PostAsJsonAsync("/api/payment-methods", createRequest);
            paymentMethodId = int.Parse(createResponse.Headers.Location!.OriginalString.Split('/')[^1]);
        }

        using var verificationClient = _factory.CreateClient();

        var deleteResponse = await verificationClient.DeleteAsync($"/api/payment-methods/{paymentMethodId}");
        var deletePayload = await deleteResponse.Content.ReadFromJsonAsync<ApiResponse<bool>>(_jsonOptions);

        Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);
        Assert.NotNull(deletePayload);
        Assert.True(deletePayload.Success);
        Assert.True(deletePayload.Result);

        var getResponse = await verificationClient.GetAsync($"/api/payment-methods/{paymentMethodId}");
        var getPayload = await getResponse.Content.ReadFromJsonAsync<ApiResponse>(_jsonOptions);

        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        Assert.NotNull(getPayload);
        Assert.False(getPayload.Success);
        Assert.Equal("not_found", getPayload.Code);
    }
}
