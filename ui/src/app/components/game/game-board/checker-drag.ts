import { CheckerArea } from 'src/app/utils/checker-area';

export class CheckerDrag {
  constructor(
    public checkerArea: CheckerArea,
    public xDown: number,
    public yDown: number,
    public fromIdx: number
  ) {}
}
