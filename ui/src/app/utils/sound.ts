export class Sound {
  click: HTMLAudioElement;
  dice: HTMLAudioElement;
  checker: HTMLAudioElement;
  checkerWood: HTMLAudioElement;
  looser: HTMLAudioElement;
  swish: HTMLAudioElement;
  warning: HTMLAudioElement;
  winner: HTMLAudioElement;
  coin: HTMLAudioElement;
  blues: HTMLAudioElement;
  tick: HTMLAudioElement;
  pianointro: HTMLAudioElement;

  private constructor() {
    this.click = new Audio();
    this.click.src = '../assets/sound/click.wav';
    this.click.load();

    this.dice = new Audio();
    this.dice.src = '../assets/sound/dice.wav';
    this.dice.load();

    this.checker = new Audio();
    this.checker.src = '../assets/sound/checker.wav';
    this.checker.load();

    this.checkerWood = new Audio();
    this.checkerWood.src = '../assets/sound/checker-wood.wav';
    this.checkerWood.load();

    this.looser = new Audio();
    this.looser.src = '../assets/sound/looser.wav';
    this.looser.load();

    this.swish = new Audio();
    this.swish.src = '../assets/sound/swish.wav';
    this.swish.load();

    this.warning = new Audio();
    this.warning.src = '../assets/sound/warning.wav';
    this.warning.load();

    this.winner = new Audio();
    this.winner.src = '../assets/sound/winner.wav';
    this.winner.load();

    this.coin = new Audio();
    this.coin.src = '../assets/sound/coin.wav';
    this.coin.preload = 'auto';
    this.coin.load();

    this.blues = new Audio();
    this.blues.src = '../assets/sound/blues.mp3';
    this.blues.preload = 'auto';
    this.blues.load();

    this.tick = new Audio();
    this.tick.src = '../assets/sound/ticktock.mp3';
    this.tick.load();
    this.tick.volume = 0.25;

    this.pianointro = new Audio();
    this.pianointro.src = '../assets/sound/pianointro.mp3';
    this.pianointro.load();
  }

  private static _singleton: Sound;
  public static get Singleton(): Sound {
    if (!this._singleton) {
      this._singleton = new Sound();
    }
    return this._singleton;
  }

  static playClick(): void {
    this.Singleton.click.play();
  }

  static playDice(): void {
    this.Singleton.dice.play();
  }

  static playChecker(): void {
    this.Singleton.checker.play();
  }

  static playCheckerWood(): void {
    this.Singleton.checkerWood.play();
  }

  static playLooser(): void {
    this.Singleton.looser.play();
  }

  static playSwish(): void {
    this.Singleton.swish.play();
  }

  static playWarning(): void {
    this.Singleton.warning.play();
  }

  static playWinner(): void {
    this.Singleton.winner.play();
  }

  static playCoin(): void {
    this.Singleton.coin.play();
  }

  static playTick(): void {
    this.Singleton.tick.play();
  }

  static playBlues(): void {
    this.Singleton.blues.volume = 1;
    this.Singleton.blues.play();
  }

  static playPianoIntro(): void {
    this.Singleton.pianointro.volume = 0.5;
    this.Singleton.pianointro.play();
  }

  static fadeSound(sound: HTMLAudioElement): void {
    const startVol = sound.volume;
    if (startVol === 0) return;
    const interval = 50; //ms
    const fadeLength = 1000;
    const fadeStep = startVol / (fadeLength / interval);
    const handle = setInterval(() => {
      let b = sound.volume;
      b -= fadeStep;
      if (b < 0) {
        b = 0;
        clearInterval(handle);
      }
      sound.volume = b;
    }, interval);
  }

  static fadeIntro() {
    Sound.fadeSound(Sound.Singleton.blues);
    Sound.fadeSound(Sound.Singleton.pianointro);
  }
}
