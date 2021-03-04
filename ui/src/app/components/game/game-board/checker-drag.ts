import { PlayerColor } from 'src/app/dto';
import { CheckerArea } from './';

export class CheckerDrag {
  constructor(
    public checkerArea: CheckerArea,
    public xDown: number,
    public yDown: number,
    public fromIdx: number,
    public color: PlayerColor
  ) {}
}
