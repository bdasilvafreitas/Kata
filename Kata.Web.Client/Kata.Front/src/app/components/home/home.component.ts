import { Component, ViewChild } from '@angular/core';
import { ListComponent } from './list/list.component';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog } from '@angular/material/dialog';
import { CreateComponent } from './create/create.component';
import { MatCommonModule } from '@angular/material/core';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [ListComponent, MatButtonModule, MatCommonModule],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss',
})
export class HomeComponent {
  @ViewChild(ListComponent) listComponent!: ListComponent;

  constructor(private dialog: MatDialog) {}

  onAdd() {
    const dialogRef = this.dialog.open(CreateComponent, { width: '80%' });

    dialogRef.afterClosed().subscribe(() => {
      this.listComponent.loadAppointments();
    });
  }
}
