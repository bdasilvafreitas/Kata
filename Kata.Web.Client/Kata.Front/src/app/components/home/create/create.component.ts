import { Component } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';
import { MatInputModule } from '@angular/material/input';
import {
  AppointmentDto,
  AppointmentService,
  TimeSlotDto,
} from '../../../services/appointment.service.generated';
import { Observable, catchError, tap, throwError } from 'rxjs';
import { MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { CommonModule } from '@angular/common';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { faker } from '@faker-js/faker';
import { environment } from '../../../environments/environment';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import moment from 'moment';

@Component({
  selector: 'app-create',
  standalone: true,
  imports: [
    CommonModule,
    MatDialogModule,
    ReactiveFormsModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatDatepickerModule,
    MatSnackBarModule,
  ],
  templateUrl: './create.component.html',
  styleUrl: './create.component.scss',
})
export class CreateComponent {
  appointmentForm: FormGroup;
  timeSlots$: Observable<TimeSlotDto[]> | undefined;
  timeSlotsForRandom: TimeSlotDto[] = [];
  isProd = environment.production;
  todayDate = new Date();
  tomorowDate = new Date();

  constructor(
    private fb: FormBuilder,
    private dialogRef: MatDialogRef<CreateComponent>,
    private appointmentService: AppointmentService,
    private snackBar: MatSnackBar
  ) {
    this.tomorowDate.setDate(this.tomorowDate.getDate() + 1);

    this.appointmentForm = this.fb.group({
      appointmentDate: ['', Validators.required],
      room: ['', Validators.required],
      timeSlot: [{ value: '', disabled: true }, Validators.required],
      firstName: [{ value: '', disabled: true }, Validators.required],
      lastName: [{ value: '', disabled: true }, Validators.required],
      dob: [{ value: '', disabled: true }, Validators.required],
      email: [
        { value: '', disabled: true },
        [Validators.required, Validators.email],
      ],
    });
  }

  ngOnInit(): void {
    this.appointmentForm.get('room')?.valueChanges.subscribe((roomNumber) => {
      this.resetFields();
      const appDate = this.appointmentForm.get('appointmentDate')?.value;

      if (appDate) {
        this.enableFields();
        this.loadAvailableTimeSlots(appDate, roomNumber);
      }
    });

    this.appointmentForm
      .get('appointmentDate')
      ?.valueChanges.subscribe((date) => {
        this.resetFields();
        if (date) {
          const roomNumber = this.appointmentForm.get('room')?.value;
          if (roomNumber) {
            this.enableFields();
            this.loadAvailableTimeSlots(date, roomNumber);
          }
        } else {
          this.disableFields();
        }
      });
  }

  private loadAvailableTimeSlots(appointmentDate: Date, roomNumber: number) {
    this.timeSlots$ = this.appointmentService
      .availableTimeSlots(appointmentDate.toLocaleDateString(), roomNumber)
      .pipe(tap((s) => (this.timeSlotsForRandom = s)));

    this.timeSlots$.subscribe();
  }

  private enableFields() {
    this.appointmentForm.get('timeSlot')?.enable();
    this.appointmentForm.get('firstName')?.enable();
    this.appointmentForm.get('lastName')?.enable();
    this.appointmentForm.get('dob')?.enable();
    this.appointmentForm.get('email')?.enable();
  }
  private disableFields() {
    this.appointmentForm.get('timeSlot')?.disable();
    this.appointmentForm.get('firstName')?.disable();
    this.appointmentForm.get('lastName')?.disable();
    this.appointmentForm.get('dob')?.disable();
    this.appointmentForm.get('email')?.disable();
  }

  private resetFields() {
    this.appointmentForm.patchValue({
      timeSlot: '',
      firstName: '',
      lastName: '',
      dob: '',
      email: '',
    });
  }

  onSubmit() {
    // console.log(this.appointmentForm.value);
    const appointmentDto: AppointmentDto = {
      timeSlotId: this.appointmentForm.get('timeSlot')?.value,
      roomNumber: this.appointmentForm.get('room')?.value,
      appointmentDate: this.appointmentForm.get('appointmentDate')?.value,
      firstName: this.appointmentForm.get('firstName')?.value,
      lastName: this.appointmentForm.get('lastName')?.value,
      dateOfBirth: this.appointmentForm.get('dob')?.value,
      email: this.appointmentForm.get('email')?.value,
    };
    this.appointmentService
      .appointmentPOST(appointmentDto)
      .pipe(
        catchError((err) => {
          this.snackBar.open(`Error: ${err.message}`, '', {
            duration: 5 * 1000,
          });
          return throwError(() => new Error(err.message));
        })
      )
      .subscribe((res) => {
        if (res) {
          this.dialogRef.close(this.appointmentForm.value);
        }
      });
  }

  onCancel() {
    this.dialogRef.close(false);
  }

  // random tools

  onRandom() {
    this.appointmentForm.patchValue({
      timeSlot: faker.helpers.arrayElement(
        this.timeSlotsForRandom.map((t) => t.id)
      ),
      firstName: faker.person.firstName(),
      lastName: faker.person.lastName(),
      dob: faker.date.birthdate(),
      email: faker.internet.email(),
    });
  }
}
