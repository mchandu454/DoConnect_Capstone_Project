using System;
using System.Security.Claims;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using DoConnect.API.DTOs;

namespace DoConnect.API.Hubs
{
    [Authorize(Roles = "Admin")]
    public class NotificationsHub : Hub
    {
        public const string AdminGroup = "admins";
        public static ConcurrentDictionary<string, byte> ConnectedAdmins { get; } = new();
        private static readonly object RecentLock = new();
        private static readonly List<NotificationDto> RecentNotifications = new();
        private const int RecentLimit = 30;

        public static void AddRecent(NotificationDto notification)
        {
            if (notification == null) return;
            lock (RecentLock)
            {
                // De-dup by Id so client doesn't show duplicates.
                RecentNotifications.RemoveAll(n => n.Id == notification.Id);
                RecentNotifications.Insert(0, notification);
                if (RecentNotifications.Count > RecentLimit)
                {
                    RecentNotifications.RemoveRange(RecentLimit, RecentNotifications.Count - RecentLimit);
                }
            }
        }

        public override async Task OnConnectedAsync()
        {
            // Because the hub is [Authorize(Roles = "Admin")], we can safely add all connected users to the admin group.
            Console.WriteLine(
                $"[SignalR] Admin connected. ConnectionId={Context.ConnectionId}, User={Context.User?.Identity?.Name}, RoleClaim={Context.User?.FindFirst(ClaimTypes.Role)?.Value}");
            ConnectedAdmins.TryAdd(Context.ConnectionId, 0);
            await Groups.AddToGroupAsync(Context.ConnectionId, AdminGroup);

            // Send last N notifications so admin still sees updates even if they connected slightly after creation.
            List<NotificationDto> snapshot;
            lock (RecentLock)
            {
                snapshot = RecentNotifications.Take(RecentLimit).ToList();
            }

            Console.WriteLine(
                $"[SignalR] Sending cached notifications to ConnectionId={Context.ConnectionId}. Count={snapshot.Count}");

            foreach (var notification in snapshot)
            {
                await Clients.Caller.SendAsync("ReceiveNotification", notification);
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            Console.WriteLine(
                $"[SignalR] Admin disconnected. ConnectionId={Context.ConnectionId}, Error={exception?.Message}");
            // Best-effort cleanup.
            ConnectedAdmins.TryRemove(Context.ConnectionId, out _);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, AdminGroup);
            await base.OnDisconnectedAsync(exception);
        }
    }
}

