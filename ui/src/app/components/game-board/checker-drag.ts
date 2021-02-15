import { Rectangle } from 'src/app/utils/rectangle';

export class CheckerDrag {
  constructor(
    public rect: Rectangle,
    public xDown: number,
    public yDown: number,
    public fromIdx: number
  ) {}
}
