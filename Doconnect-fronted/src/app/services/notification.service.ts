import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { BehaviorSubject } from 'rxjs';
import { environment } from '../../environments/environment';
import { Notification } from '../model';

@Injectable({
  providedIn: 'root',
})
export class NotificationService {
  private hubUrl = `${environment.hubUrl}/hubs/notifications`;

  private hubConnection?: signalR.HubConnection;

  private notificationsSubject = new BehaviorSubject<Notification[]>([]);
  notifications$ = this.notificationsSubject.asObservable();

  private unreadCountSubject = new BehaviorSubject<number>(0);
  unreadCount$ = this.unreadCountSubject.asObservable();

  async connect(): Promise<void> {
    const token = localStorage.getItem('token');
    if (!token) return;

    // Avoid creating multiple connections.
    if (
      this.hubConnection &&
      (this.hubConnection.state === signalR.HubConnectionState.Connected ||
        this.hubConnection.state === signalR.HubConnectionState.Connecting)
    ) {
      return;
    }

    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(this.hubUrl, {
        accessTokenFactory: () => localStorage.getItem('token') || '',
        // Avoid negotiate step (which can return 404 in your setup) and use WebSocket directly.
        transport: signalR.HttpTransportType.WebSockets,
        skipNegotiation: true,
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection.onclose((err) => {
      console.warn('[SignalR] Connection closed:', err?.message ?? err);
      // If the hub is closed unexpectedly, force a reconnect.
      this.hubConnection = undefined;
      const role = localStorage.getItem('role') || '';
      const token = localStorage.getItem('token');
      if (token && role === 'Admin') {
        void this.connect();
      }
    });
    this.hubConnection.onreconnecting(() => {
      console.warn('[SignalR] Reconnecting...');
    });
    this.hubConnection.onreconnected((connectionId) => {
      console.warn('[SignalR] Reconnected. connectionId:', connectionId);
    });

    this.hubConnection.on('ReceiveNotification', (notification: any) => {
      console.log('[SignalR] ReceiveNotification payload:', notification);
      this.addNotification(notification);
    });

    try {
      console.log('[SignalR] Connecting to hub:', this.hubUrl);
      await this.hubConnection.start();
      console.log('[SignalR] Connected to notifications hub');
    } catch (e) {
      console.error('Failed to connect to notifications hub', e);
    }
  }

  markAllAsRead(): void {
    const updated = this.notificationsSubject.value.map((n) => ({ ...n, read: true }));
    this.notificationsSubject.next(updated);
    this.unreadCountSubject.next(0);
  }

  markAsRead(id: string): void {
    if (!id) return;
    const updated = this.notificationsSubject.value.map((n) => {
      if (n.id !== id) return n;
      return { ...n, read: true };
    });
    this.notificationsSubject.next(updated);
    this.unreadCountSubject.next(updated.filter((n) => !n.read).length);
  }

  disconnect(): void {
    try {
      this.hubConnection?.stop();
    } catch {
      // best-effort
    }

    this.hubConnection = undefined;
    this.notificationsSubject.next([]);
    this.unreadCountSubject.next(0);
  }

  private addNotification(raw: any): void {
    // Backend may serialize C# properties with different casing (Id vs id).
    const normalized: Notification = {
      id: String(raw?.id ?? raw?.Id ?? ''),
      message: String(raw?.message ?? raw?.Message ?? ''),
      type: (raw?.type ?? raw?.Type ?? 'question') as any,
      timestamp: String(raw?.timestamp ?? raw?.Timestamp ?? ''),
      read: Boolean(raw?.read ?? raw?.Read ?? false),
    };

    if (!normalized.id || !normalized.message) {
      console.warn('[SignalR] Normalized notification is missing fields:', normalized, raw);
    }

    const current = this.notificationsSubject.value;
    const withoutDup = current.filter((n) => n.id !== normalized.id);

    const next = [normalized, ...withoutDup].slice(0, 30);
    this.notificationsSubject.next(next);
    this.unreadCountSubject.next(next.filter((n) => !n.read).length);
  }
}

