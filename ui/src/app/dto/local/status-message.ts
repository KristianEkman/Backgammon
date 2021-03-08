export class StatusMessage {
  constructor(
    public text: string,
    public level: MessageLevel,
    public startTimerSec?: number
  ) {}

  static getDefault(): StatusMessage {
    return new StatusMessage('', MessageLevel.info);
  }

  static info(text: string): StatusMessage {
    return new StatusMessage(text, MessageLevel.info);
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
