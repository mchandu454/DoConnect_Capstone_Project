import { Injectable } from '@angular/core';
import { CanActivate, Router, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {

  constructor(private router: Router) {}

  canActivate(_route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {

    const token = localStorage.getItem('token');
    const role = localStorage.getItem('role');

    // 1️⃣ If not logged in
    if(!token){
      alert("Please login first");
      this.router.navigate(['/login']);
      return false;
    }

    // 2️⃣ If admin route (dashboard or any /admin/*)
    const targetUrl = state.url;
    const isAdminRoute = targetUrl.includes('admin-dashboard') || targetUrl.includes('/admin/');

    if (isAdminRoute && role !== 'Admin') {
      alert("Access denied. Admin only.");
      this.router.navigate(['/dashboard']);
      return false;
    }

    return true;
  }

}