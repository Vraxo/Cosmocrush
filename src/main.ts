import './styles.css';
import { Game } from './game/Game';

const canvas = document.querySelector<HTMLCanvasElement>('#game-canvas');

if (!canvas) {
  throw new Error('Cosmocrush could not find its game canvas.');
}

const game = new Game(canvas);
game.start();

if (import.meta.env.DEV) {
  Object.assign(window, { cosmocrush: game });
}