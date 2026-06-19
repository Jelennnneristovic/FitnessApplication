import { Component, inject, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { UserRole } from '../../core/models/user.model';

@Component({
  selector: 'app-home',
  imports: [],
  template: '<p>Preusmeravanje...</p>'
})
export class Home implements OnInit {
  private authService = inject(AuthService);
  private router = inject(Router);

  ngOnInit(): void {
    const role = this.authService.getUserRole();

    switch (role) {
      case UserRole.Admin:
        this.router.navigate(['/admin']);
        break;
      case UserRole.Trainer:
        this.router.navigate(['/trainer']);
        break;
      case UserRole.Client:
        this.router.navigate(['/client']);
        break;
      default:
        this.router.navigate(['/login']);
    }
  }
}