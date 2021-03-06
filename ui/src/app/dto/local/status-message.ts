export class StatusMessage {
  constructor(
    public text: string,
    public level: MessageLevel,
    public startTimerSec?: number,
    timesUp?: () => void
  ) {
    if (!startTimerSec || !timesUp) {
      return;
    }
    if (startTimerSec > 0) {
      const handle = setTimeout(() => {
        clearTimeout(handle);
        timesUp();
      }, startTimerSec * 1000);
    }
  }

  static getDefault(): StatusMessage {
    return new StatusMessage('', MessageLevel.info);
  }

  static info(text: string): StatusMessage {
    return new StatusMessage(text, MessageLevel.info);
  }

  static timer(text: string, secs: number, timesUp: () => void): StatusMessage {
    return new StatusMessage(text, MessageLevel.info, secs, timesUp);
  }

  static error(text: string): StatusMessage {
    return new StatusMessage(text, MessageLevel.error);
  }

  static warning(text: string): StatusMessage {
    return new StatusMessage(text, MessageLevel.warning);
  }
}

export enum MessageLevel {
  info,
  warning,
  error
}
