using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ServiceContract;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Service
{
    public class TelegramPerformanceAlertService : IPerformanceAlertService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<TelegramPerformanceAlertService> _logger;
        private readonly string? _botToken;
        private readonly string? _chatId;
        private readonly bool _isConfigured;

        public TelegramPerformanceAlertService(
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<TelegramPerformanceAlertService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _botToken = configuration["Telegram:BotToken"];
            _chatId = configuration["Telegram:ChatId"];

            _isConfigured = !string.IsNullOrEmpty(_botToken) && !string.IsNullOrEmpty(_chatId);

            if (!_isConfigured)
            {
                _logger.LogWarning("Telegram BotToken or ChatId is missing in configuration.");
            }
        }

        public async Task SendSlowEndpointAlertAsync(string endpoint, long durationMs)
        {
            if (!_isConfigured) return;

            var message =
                $"⚠️ <b>Slow Endpoint Alert!</b>\n\n" +
                $"• <b>Endpoint:</b> <code>{HtmlEncode(endpoint)}</code>\n" +
                $"• <b>Duration:</b> <code>{durationMs} ms</code>\n" +
                $"• <b>Time (UTC):</b> <code>{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}</code>";

            await SendTelegramRequestAsync(message);
        }

        public async Task SendErrorAlertAsync(string title, string errorDetails)
        {
            if (!_isConfigured) return;

            var message =
                $"🚨 <b>{HtmlEncode(title)}</b>\n\n" +
                $"<code>{HtmlEncode(errorDetails)}</code>\n\n" +
                $"• <b>Time (UTC):</b> <code>{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}</code>";

            await SendTelegramRequestAsync(message);
        }

        private async Task SendTelegramRequestAsync(string messageHtml)
        {
            try
            {
                var payload = new
                {
                    chat_id = _chatId,
                    text = messageHtml,
                    parse_mode = "HTML"
                };

                // Construct full target URL using the bot token from User Secrets / IConfiguration
                var requestUrl = $"https://api.telegram.org/bot{_botToken}/sendMessage";

                var response = await _httpClient.PostAsJsonAsync(requestUrl, payload);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Failed to send Telegram alert. Status: {StatusCode}, Error: {Error}",
                        response.StatusCode, errorContent);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An exception occurred while sending Telegram alert.");
            }
        }

        // Helper using System.Net.WebUtility safely
        private static string HtmlEncode(string value) => WebUtility.HtmlEncode(value ?? string.Empty);
    }
}