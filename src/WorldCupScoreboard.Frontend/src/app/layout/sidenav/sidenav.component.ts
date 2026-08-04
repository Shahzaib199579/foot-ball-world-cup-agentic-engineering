import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatListModule } from '@angular/material/list';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-sidenav',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatSidenavModule,
    MatListModule,
    MatToolbarModule,
    MatIconModule,
    MatButtonModule
  ],
  template: `
    <div class="app-layout">
      <!-- Main Top Toolbar -->
      <mat-toolbar class="main-toolbar">
        <div class="toolbar-brand">
          <mat-icon class="brand-logo">sports_soccer</mat-icon>
          <span class="brand-title">World Cup Scoreboard</span>
        </div>
        <span class="toolbar-spacer"></span>
        <span class="edition-badge">Live Edition</span>
      </mat-toolbar>

      <mat-sidenav-container class="sidenav-container">
        <!-- Left Side Navigation -->
        <mat-sidenav mode="side" opened class="left-nav">
          <div class="nav-header">
            <span class="nav-header-text">NAVIGATION</span>
          </div>

          <mat-nav-list class="nav-list">
            <a mat-list-item routerLink="/summary" routerLinkActive="active-link" class="nav-item">
              <mat-icon matListItemIcon class="nav-icon">bar_chart</mat-icon>
              <span matListItemTitle class="nav-text">Summary</span>
            </a>

            <a mat-list-item routerLink="/history" routerLinkActive="active-link" class="nav-item">
              <mat-icon matListItemIcon class="nav-icon">history</mat-icon>
              <span matListItemTitle class="nav-text">History</span>
            </a>

            <a mat-list-item routerLink="/matches" routerLinkActive="active-link" class="nav-item">
              <mat-icon matListItemIcon class="nav-icon">sports</mat-icon>
              <span matListItemTitle class="nav-text">Matches</span>
            </a>
          </mat-nav-list>
        </mat-sidenav>

        <!-- Main Content Area -->
        <mat-sidenav-content class="main-content">
          <div class="content-wrapper">
            <router-outlet></router-outlet>
          </div>
        </mat-sidenav-content>
      </mat-sidenav-container>
    </div>
  `,
  styles: [`
    .app-layout {
      display: flex;
      flex-direction: column;
      height: 100vh;
      width: 100vw;
      overflow: hidden;
      background-color: #f8fafc;
    }
    .main-toolbar {
      background: linear-gradient(135deg, #002244 0%, #003366 100%);
      color: #ffffff;
      height: 64px;
      box-shadow: 0 2px 10px rgba(0, 0, 0, 0.15);
      z-index: 10;
      padding: 0 24px;
    }
    .toolbar-brand {
      display: flex;
      align-items: center;
      gap: 12px;
    }
    .brand-logo {
      font-size: 28px;
      width: 28px;
      height: 28px;
      color: #60a5fa;
    }
    .brand-title {
      font-size: 1.35rem;
      font-weight: 700;
      letter-spacing: -0.02em;
      color: #ffffff;
    }
    .toolbar-spacer {
      flex: 1 1 auto;
    }
    .edition-badge {
      background: rgba(255, 255, 255, 0.15);
      backdrop-filter: blur(4px);
      padding: 4px 12px;
      border-radius: 16px;
      font-size: 0.8rem;
      font-weight: 600;
      letter-spacing: 0.05em;
      text-transform: uppercase;
      border: 1px solid rgba(255, 255, 255, 0.2);
    }
    .sidenav-container {
      flex: 1;
      height: calc(100vh - 64px);
    }
    .left-nav {
      width: 240px;
      background: #ffffff;
      border-right: 1px solid #e2e8f0;
      box-shadow: 2px 0 8px rgba(0, 0, 0, 0.02);
    }
    .nav-header {
      padding: 24px 20px 12px 20px;
    }
    .nav-header-text {
      font-size: 0.75rem;
      font-weight: 700;
      color: #94a3b8;
      letter-spacing: 0.1em;
    }
    .nav-list {
      padding: 0 12px;
    }
    .nav-item {
      border-radius: 8px !important;
      margin-bottom: 6px !important;
      transition: background-color 0.2s ease, color 0.2s ease;
    }
    .nav-icon {
      color: #64748b;
    }
    .nav-text {
      font-size: 0.95rem;
      font-weight: 600;
      color: #334155;
    }
    .nav-item:hover {
      background-color: #e0e7ff !important;
      .nav-icon, .nav-text {
        color: #0a369d !important;
      }
    }
    .active-link {
      background-color: #0a369d !important;
      .nav-icon, .nav-text {
        color: #ffffff !important;
      }
    }
    .main-content {
      background-color: #f8fafc;
      overflow-y: auto;
    }
    .content-wrapper {
      padding: 32px 40px;
    }
  `]
})
export class SidenavComponent {}
