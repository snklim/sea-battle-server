import player from './player'

export default class Game {
    nextPlayer = 1;
    status = -1;

    players = [];

    start() {

        this.nextPlayer = Math.round(Math.random());
        this.status = -1;

        this.players = []

        this.players.push(new player())
        this.players.push(new player())

        this.players[0].init();
        this.players[1].init();

        return {
            nextPlayer: this.nextPlayer,
            nextMove: [
                this.players[0].generateNextMoveFn(),
                this.players[1].generateNextMoveFn()
            ],
            fields: [
                this.players[0].field,
                this.players[1].field
            ]
        }
    }

    move({ x, y, fieldIndex }) {
        let changes = {
            valid: false,
            x,
            y,
            cells: [],
            nextPlayer: this.nextPlayer,
            nextMove: null,
            fieldIndex,
            status: this.status
        };

        let player = this.players[fieldIndex]

        if (this.status === -1 && changes.nextPlayer === fieldIndex && player.availableMoves.findIndex(item => item.x === x && item.y === y) !== -1) {
            changes.valid = true;
            changes.nextPlayer = player.shotFn({ x, y, ...changes }) ? fieldIndex : (fieldIndex + 1) % 2;
            player.updateNextMovesFn({ x, y })
            player.drawBorderFn({ x, y, ...changes })
            player.updateAvailableMovesFn({ x, y })
            player.generateNextMovesFn({ x, y })
            changes.nextMove = player.generateNextMoveFn()
            this.nextPlayer = changes.nextPlayer

            if (player.getAvailableShips() === 0)
                this.status = fieldIndex

            changes.status = this.status;
        }

        return changes
    }
}