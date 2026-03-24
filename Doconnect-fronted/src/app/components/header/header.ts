import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';
import { filter } from 'rxjs/operators';
import { Subscription } from 'rxjs';
import { Notification } from '../../model';
import { NotificationService } from '../../services/notification.service';

@Component({
  selector: 'app-header',
  standalone: false,
  templateUrl: './header.html',
  styleUrl: './header.css'
})
export class HeaderComponent implements OnInit, OnDestroy {

  showHeader = false;
  isAdmin = false;
  dashboardLink = '/dashboard';
  menuOpen = false;
  searchQuery = '';
  private sub: Subscription | null = null;

  notifications: Notification[] = [];
  unreadCount = 0;
  notifOpen = false;
  selectedNotification: Notification | null = null;

  private notificationsSub: Subscription | null = null;
  private unreadCountSub: Subscription | null = null;

  constructor(
    private router: Router,
    private notificationService: NotificationService
  ) {}

  ngOnInit() {
    this.updateState(this.router.url);

    this.notificationsSub = this.notificationService.notifications$.subscribe((n) => {
      this.notifications = n;
    });
    this.unreadCountSub = this.notificationService.unreadCount$.subscribe((c) => {
      this.unreadCount = c;
    });

    if (this.isAdmin) {
      void this.notificationService.connect();
    }

    this.sub = this.router.events
      .pipe(filter((e): e is NavigationEnd => e instanceof NavigationEnd))
      .subscribe(e => {
        this.updateState(e.url);
        this.closeMenu();
      });
  }

  ngOnDestroy() {
    this.sub?.unsubscribe();
    this.notificationsSub?.unsubscribe();
    this.unreadCountSub?.unsubscribe();
  }

  private updateState(url: string) {
    this.showHeader = !url.includes('/login') && !url.includes('/register');
    const role = localStorage.getItem('role') || '';
    this.isAdmin = role === 'Admin';
    this.dashboardLink = this.isAdmin ? '/admin-dashboard' : '/dashboard';

    if (this.isAdmin && this.showHeader) {
      void this.notificationService.connect();
    } else {
      this.notifOpen = false;
    }
  }

  toggleMenu() {
    this.menuOpen = !this.menuOpen;
  }

  closeMenu() {
    this.menuOpen = false;
  }

  onSearch() {
    const q = (this.searchQuery || '').trim();
    if (q) {
      this.router.navigate(['/search'], { queryParams: { q } });
      this.closeMenu();
    }
  }

  logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('role');
    localStorage.removeItem('username');
    localStorage.removeItem('userId');
    this.notificationService.disconnect();
    this.notifOpen = false;
    this.router.navigate(['/login']);
  }

  toggleNotifications(evt?: Event) {
    evt?.stopPropagation();
    const opening = !this.notifOpen;
    this.notifOpen = !this.notifOpen;

    if (opening) {
      // When opening dropdown, keep current read/unread state.
      this.selectedNotification = null;
    }
  }

  closeNotifications() {
    this.notifOpen = false;
    this.selectedNotification = null;
  }

  onNotificationClick(n: Notification, evt: Event) {
    evt.stopPropagation();
    this.selectedNotification = n;
    // Mark ONLY this notification as read after clicking it.
    this.notificationService.markAsRead(n.id);
  }
}
