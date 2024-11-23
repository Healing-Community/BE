using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Encodings.Web;
using Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace PRH_ChatService_API;

public class WebSocketHandler
{
    private static readonly ConcurrentDictionary<string, List<WebSocket>> sessions = new();
    private readonly IMongoRepository<ChatMessages> _chatMessagesRepository;

    // JSON Serializer Options to keep Unicode characters intact
    private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    public WebSocketHandler(IMongoRepository<ChatMessages> chatMessagesRepository)
    {
        _chatMessagesRepository = chatMessagesRepository;
    }

    public async Task HandleChatSessionAsync(HttpContext context, WebSocket webSocket)
    {
        // Retrieve userId and partnerId from query parameters
        var userId = context.Request.Query["userId"].ToString();
        var partnerId = context.Request.Query["partnerId"].ToString();

        // Validate userId and partnerId
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(partnerId))
        {
            await SendErrorMessageAndClose(webSocket, "Thiếu thông tin userId hoặc partnerId");
            return;
        }

        if (userId == partnerId)
        {
            await SendErrorMessageAndClose(webSocket, "id người gửi và người nhận không thể giống nhau");
            return;
        }

        // Generate sessionId
        var sessionId = GenerateSessionId(userId, partnerId);

        // Add or update WebSocket to the session
        sessions.AddOrUpdate(sessionId, new List<WebSocket> { webSocket }, (key, existingList) =>
        {
            lock (existingList)
            {
                if (!existingList.Contains(webSocket))
                    existingList.Add(webSocket);
            }
            return existingList;
        });

        // Fetch old messages from MongoDB
        var chatMessages = await _chatMessagesRepository.GetManyByPropertyAsync(c => c.SessionId == sessionId) ?? new List<ChatMessages>();
        foreach (var message in chatMessages)
        {
            var messageBuffer = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message, JsonOptions));
            if (webSocket.State == WebSocketState.Open)
            {
                await webSocket.SendAsync(new ArraySegment<byte>(messageBuffer), WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }

        // Handle incoming messages
        var buffer = new byte[1024 * 4];
        WebSocketReceiveResult result;

        try
        {
            result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        }
        catch (WebSocketException ex)
        {
            Console.WriteLine($"WebSocket error during receive: {ex.Message}");
            return;
        }

        while (!result.CloseStatus.HasValue)
        {
            var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
            var content = ParseMessageContent(message);

            if (content == null)
            {
                Console.WriteLine("Received invalid message format. Closing connection...");
                // Gửi lỗi và đóng kết nối
                await SendErrorMessageAndClose(webSocket, "Invalid message format");
                return;
            }

            // Save new message to database
            var chatMessage = new ChatMessages(userId, partnerId, content, sessionId);
            await _chatMessagesRepository.Create(chatMessage);

            // Broadcast the message to all WebSockets in the session
            foreach (var socket in sessions[sessionId].ToList()) // Avoid concurrency issues with ToList()
            {
                if (socket.State == WebSocketState.Open && socket != webSocket)
                {
                    try
                    {
                        var messageBuffer = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new { content }, JsonOptions));
                        await socket.SendAsync(new ArraySegment<byte>(messageBuffer), WebSocketMessageType.Text, true, CancellationToken.None);
                    }
                    catch (WebSocketException sendEx)
                    {
                        Console.WriteLine($"Broadcast error: {sendEx.Message}");
                        // Remove the socket if sending fails
                        sessions[sessionId].Remove(socket);
                    }
                }
            }

            // Wait for the next message
            try
            {
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }
            catch (WebSocketException ex)
            {
                Console.WriteLine($"WebSocket error during receive: {ex.Message}");
                break;
            }
        }

        // Cleanup on disconnection
        if (sessions.TryGetValue(sessionId, out var webSocketList))
        {
            lock (webSocketList)
            {
                webSocketList.Remove(webSocket);
                if (webSocketList.Count == 0)
                {
                    sessions.TryRemove(sessionId, out _);
                }
            }
        }

        if (webSocket.State == WebSocketState.Open)
        {
            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }
    }

    private static async Task SendErrorMessageAndClose(WebSocket webSocket, string errorMessage)
    {
        if (webSocket.State == WebSocketState.Open)
        {
            var errorBuffer = Encoding.UTF8.GetBytes(errorMessage);
            await webSocket.SendAsync(new ArraySegment<byte>(errorBuffer), WebSocketMessageType.Text, true, CancellationToken.None);
            await webSocket.CloseAsync(WebSocketCloseStatus.PolicyViolation, errorMessage, CancellationToken.None);
        }
    }

    private static string GenerateSessionId(string userId, string partnerId)
    {
        return string.Compare(userId, partnerId, StringComparison.Ordinal) < 0
            ? $"{userId}-{partnerId}"
            : $"{partnerId}-{userId}";
    }

    private static string ParseMessageContent(string message)
    {
        try
        {
            var jsonDoc = JsonDocument.Parse(message);
            return jsonDoc.RootElement.GetProperty("content").GetString();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error parsing message: {ex.Message}");
            return null;
        }
    }
}
