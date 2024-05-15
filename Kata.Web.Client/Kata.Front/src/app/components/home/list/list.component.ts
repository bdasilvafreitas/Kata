import { MatSnackBar } from '@angular/material/snack-bar';
import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';
import { MatSort, MatSortModule } from '@angular/material/sort';
import { CommonModule } from '@angular/common';
import { Observable, catchError, tap, throwError } from 'rxjs';
import {
  AppointmentDto,
  AppointmentService,
} from '../../../services/appointment.service.generated';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { ConfirmDialogComponent } from '../../shared/confirm-dialog/confirm-dialog.component';
import { RoomNamePipe } from '../../shared/pipes/room-name.pipe';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-list',
  standalone: true,
  imports: [
    CommonModule,
    MatDialogModule,
    MatIconModule,
    MatButtonModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    RoomNamePipe,
  ],
  templateUrl: './list.component.html',
  styleUrl: './list.component.scss',
})
export class ListComponent implements OnInit, AfterViewInit {
  @ViewChild(MatPaginator) paginator: MatPaginator | undefined;
  @ViewChild(MatSort) sort: MatSort | undefined;

  displayedColumns: string[] = [
    'name',
    'slot',
    'roomNumber',
    'dateOfBirth',
    'email',
    'createdAt',
    'actions',
  ];

  dataSource = new MatTableDataSource<AppointmentDto>([]);
  appointments: AppointmentDto[] = [];
  constructor(
    private appointmentService: AppointmentService,
    private snackBar: MatSnackBar,
    private dialog: MatDialog
  ) {}

  ngAfterViewInit(): void {
    this.loadAppointments();

    if (this.sort) {
      this.dataSource.sort = this.sort;
    }
    if (this.paginator) {
      this.dataSource.paginator = this.paginator;
    }
  }

  ngOnInit(): void {}

  onDelete(id: string) {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      width: '500px',
      data: {
        title: 'Confirm Deletion',
        message: 'Are you sure you want to delete this appointment?',
      },
    });

    dialogRef.afterClosed().subscribe((result: boolean) => {
      if (result) {
        this.appointmentService
          .appointmentDELETE(id)
          .pipe(
            tap((res) =>
              this.snackBar.open(`Deletion: OK`, '', { duration: 3 * 1000 })
            ),
            tap(() => this.loadAppointments()),
            catchError((err) => {
              this.snackBar.open(`Error: ${err.message}`, '', {
                duration: 5 * 1000,
              });
              return throwError(() => new Error(err.message));
            })
          )
          .subscribe();
      }
    });
  }

  loadAppointments() {
    this.appointmentService
      .appointmentAll()
      .pipe(tap((x) => (this.appointments = x)))
      .subscribe((res) => {
        this.dataSource.data = res;
        this.dataSource.filter = '';
      });
  }
}
