import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';
import { filter } from 'rxjs/operators';
import { Subscription } from 'rxjs';

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

  constructor(private router: Router) {}

  ngOnInit() {
    this.updateState(this.router.url);
    this.sub = this.router.events
      .pipe(filter((e): e is NavigationEnd => e instanceof NavigationEnd))
      .subscribe(e => {
        this.updateState(e.url);
        this.closeMenu();
      });
  }

  ngOnDestroy() {
    this.sub?.unsubscribe();
  }

  private updateState(url: string) {
    this.showHeader = !url.includes('/login') && !url.includes('/register');
    const role = localStorage.getItem('role') || '';
    this.isAdmin = role === 'Admin';
    this.dashboardLink = this.isAdmin ? '/admin-dashboard' : '/dashboard';
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
    this.router.navigate(['/login']);
  }
}
