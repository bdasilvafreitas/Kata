import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'roomName',
  standalone: true,
})
export class RoomNamePipe implements PipeTransform {
  transform(value: number): string {
    if (!value) {
      return '';
    }

    return `Room ${value}`;
  }
}
