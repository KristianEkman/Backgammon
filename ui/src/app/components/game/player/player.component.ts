import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, Input } from '@angular/core';
import { PlayerDto } from 'src/app/dto';

@Component({
  selector: 'app-player',
  standalone: true,
  templateUrl: './player.component.html',
  styleUrls: ['./player.component.scss'],
  imports: [CommonModule],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class PlayerComponent {
  constructor() {}

  ainaUrl = '/assets/images/aina.png';
  @Input() playerDto?: PlayerDto;
  @Input() doubling: number | null = null;

  getPhotoUrl(): string {
    if (!this.playerDto?.photoUrl) return '';

    return this.playerDto.photoUrl === 'aina'
      ? this.ainaUrl
      : this.playerDto.photoUrl;
  }

  getInitials(): string {
    if (!this.playerDto?.name) return '';
    const names = this.playerDto.name.split(' ');
    let initials = '';
    for (let i = 0; i < names.length; i++) {
      const name = names[i];
      if (i < 2) initials += name.substr(0, 1);
    }
    return initials;
  }
}
