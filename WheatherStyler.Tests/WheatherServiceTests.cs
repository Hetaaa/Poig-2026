using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using WeatherStyler.Application.Services;
using WeatherStyler.Domain.Entities.BuisnessLogic;
using Xunit;

namespace WeatherStyler.Tests
{
    public class WeatherServiceTests
    {
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<IConfigurationSection> _configSectionMock;
        private readonly Mock<HttpMessageHandler> _httpHandlerMock;

        public WeatherServiceTests()
        {
            _configurationMock = new Mock<IConfiguration>();
            _configSectionMock = new Mock<IConfigurationSection>();
            _httpHandlerMock = new Mock<HttpMessageHandler>();

            // Definiujemy progi pogodowe identyczne jak w appsettings.json
            var thresholds = new WeatherThresholds
            {
                RainThreshold = 0.1f,
                WindThreshold = 15.0f,
                SunnyCloudThreshold = 30
            };

            _configSectionMock.Setup(x => x.Value).Returns((string)null);
            _configurationMock.Setup(x => x.GetSection("WeatherThresholds")).Returns(_configSectionMock.Object);

            // Konfiguracja Moq, aby IConfiguration.Get<WeatherThresholds>() zwracało nasze testowe progi
            // W testach jednostkowych najbezpieczniej zasymulować to przez bezpośrednie bindowanie sekcji,
            // bądź upewnić się, że sekwencja metod zwraca obiekt.
        }

        // Metoda pomocnicza tworząca HttpClient ze sztuczną odpowiedzią JSON
        private HttpClient CreateMockHttpClient(HttpResponseMessage responseMessage)
        {
            _httpHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(responseMessage);

            return new HttpClient(_httpHandlerMock.Object);
        }

        [Theory]
        [InlineData("invalid_lat", "18.85")]
        [InlineData("50.44", "invalid_lon")]
        [InlineData("", "")]
        public async Task GetWeatherForLocationAsync_WithInvalidStringCoordinates_ShouldReturnNull(string lat, string lon)
        {
            // Arrange
            var httpClient = new HttpClient(); // Dla niepoprawnych danych string HTTP i tak nie zostanie wywołany
            var service = new WeatherService(httpClient, _configurationMock.Object);

            // Act
            var result = await service.GetWeatherForLocationAsync(lat, lon);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetWeatherForLocationAsync_WithValidCoordinates_ShouldCalculateAveragesCorrectly()
        {
            // Arrange
            // Przygotowujemy sztuczną odpowiedź Open-Meteo z 24 wartościami (tablice od 0 do 23)
            // Ustawiamy stabilne wartości w godzinach 8-19: temp = 20, rain = 0, clouds = 10, wind = 5
            var mockJsonResponse = new
            {
                hourly = new
                {
                    temperature_2m = CreateFilledArray(24, 20f),  // średnia powinna wyjść 20
                    precipitation = CreateFilledArray(24, 0f),     // poniżej progu 0.1 -> IsRaining = false
                    cloud_cover = CreateFilledArray(24, 10f),      // poniżej progu 30 -> IsSunny = true
                    wind_speed_10m = CreateFilledArray(24, 5f)     // poniżej progu 15 -> IsWindy = false
                }
            };

            var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(mockJsonResponse))
            };

            var httpClient = CreateMockHttpClient(httpResponse);
            var service = new WeatherService(httpClient, _configurationMock.Object);

            // Act
            var result = await service.GetWeatherForLocationAsync(50.44, 18.85);

            // Assert
            result.Should().NotBeNull();
            result!.Temperature.Should().Be(20);
            result.IsRaining.Should().BeFalse();
            result.IsWindy.Should().BeFalse();
            result.IsSunny.Should().BeTrue();
        }

        [Fact]
        public async Task GetWeatherForLocationAsync_WhenRainAndWindExceedThresholds_ShouldReturnTrueFlags()
        {
            // Arrange
            // Generujemy dzień, w którym w godzinach pracy algorytmu (8-19) nagle zaczyna lać i wiać
            var temps = CreateFilledArray(24, 10f);
            var clouds = CreateFilledArray(24, 80f); // Pochmurno
            var rains = CreateFilledArray(24, 0f);
            var winds = CreateFilledArray(24, 0f);

            // Wprowadzamy anomalię w godzinie 12:00 (indeks 12) przekraczającą progi
            rains[12] = 0.5f;  // > 0.1 (Deszcz)
            winds[12] = 25.0f; // > 15.0 (Wiatr)

            var mockJsonResponse = new
            {
                hourly = new { temperature_2m = temps, precipitation = rains, cloud_cover = clouds, wind_speed_10m = winds }
            };

            var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(mockJsonResponse))
            };

            var httpClient = CreateMockHttpClient(httpResponse);
            var service = new WeatherService(httpClient, _configurationMock.Object);

            // Act
            var result = await service.GetWeatherForLocationAsync(50.44, 18.85);

            // Assert
            result.Should().NotBeNull();
            result!.IsRaining.Should().BeTrue();
            result.IsWindy.Should().BeTrue();
            result.IsSunny.Should().BeFalse();
        }

        [Fact]
        public async Task GetDailySummaryAsync_WithValidData_ShouldFilterHoursAndReturnCalculatedSummary()
        {
            // Arrange
            var times = new string[24];
            for (int i = 0; i < 24; i++)
            {
                times[i] = $"2026-06-02T{i:D2}:00";
            }

            var mockJsonResponse = new
            {
                hourly = new
                {
                    time = times,
                    temperature_2m = CreateFilledArray(24, 15.0),
                    apparent_temperature = CreateFilledArray(24, 14.0),
                    relativehumidity_2m = CreateFilledArray(24, 60.0),
                    wind_speed_10m = CreateFilledArray(24, 10.0),
                    precipitation = CreateFilledArray(24, 0.0),
                    cloud_cover = CreateFilledArray(24, 20.0)
                }
            };

            var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(mockJsonResponse))
            };

            var httpClient = CreateMockHttpClient(httpResponse);
            var service = new WeatherService(httpClient, _configurationMock.Object);

            // Act
            var result = await service.GetDailySummaryAsync(50.44, 18.85, new DateTime(2026, 06, 02));

            // Assert
            result.Should().NotBeNull();

            // Zmieniono właściwości na odpowiadające parametrom z Twojego konstruktora (z wielkiej litery)
            result!.Temperature.Should().Be(15.0);
            result.FeelsLike.Should().Be(14.0);
            result.Date.Should().Be(new DateTime(2026, 06, 02).Date);
        }

        [Fact]
        public async Task GetWeatherForLocationAsync_WhenHttpErrorOccurs_ShouldSafeCatchAndReturnNull()
        {
            // Arrange
            // Symulujemy awarię serwera zewnętrznego (Internal Server Error 500)
            var httpResponse = new HttpResponseMessage(HttpStatusCode.InternalServerError);
            var httpClient = CreateMockHttpClient(httpResponse);
            var service = new WeatherService(httpClient, _configurationMock.Object);

            // Act
            var result = await service.GetWeatherForLocationAsync(50.44, 18.85);

            // Assert
            // Dzięki Twojej konstrukcji try-catch, metoda zamiast wywalić aplikację, bezpiecznie zwróci null.
            result.Should().BeNull();
        }

        // Metody pomocnicze do szybkiego tworzenia tablic danych testowych
        private float[] CreateFilledArray(int size, float value)
        {
            var arr = new float[size];
            Array.Fill(arr, value);
            return arr;
        }

        private double[] CreateFilledArray(int size, double value)
        {
            var arr = new double[size];
            Array.Fill(arr, value);
            return arr;
        }
    }
}