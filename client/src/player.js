export default class Player {
    availableMoves = [];
    nextPossibleMoves = [];
    prevSuccessfullMoves = [];
    field = [];
    ships = [];

    shipsTypes = [4, 3, 3, 2, 2, 2, 1, 1, 1, 1]

    placeShip({ length }) {

        let max = 10;
        let attempts = 100;
        let placed = false;

        while (0 < attempts--) {
            let x = Math.round(Math.random() * 9);
            let y = Math.round(Math.random() * 9);

            let direction = Math.round(Math.random());
            let border = [];
            let ship = [];

            if (direction === 0) {

                if (x + length > max)
                    x -= x + length - max;

                for (let k = -1; k < 2; k++)
                    for (let i = -1; i < length + 1; i++)
                        if (x + i < max && y + k < max && y + k >= 0 && x + i >= 0)
                            if ((i >= 0 && i < length && k !== 0) || i < 0 || i >= length)
                                border.push({ x: x + i, y: y + k })

                for (let i = 0; i < length; i++)
                    ship.push({ x: x + i, y: y, type: 'O' })
            }
            else {

                if (y + length > max)
                    y -= y + length - max

                for (let k = -1; k < 2; k++)
                    for (let i = -1; i < length + 1; i++)
                        if (y + i < max && x + k < max && x + k >= 0 && y + i >= 0)
                            if ((i >= 0 && i < length && k !== 0) || i < 0 || i === length)
                                border.push({ x: x + k, y: y + i })

                for (let i = 0; i < length; i++)
                    ship.push({ x: x, y: y + i, type: 'O' })
            }

            let canPlace = true;

            for (let i = 0; i < ship.length; i++) {
                let cell = ship[i];
                if (this.field[cell.x][cell.y].type === 'O') {
                    canPlace = false;
                    break;
                }
            }

            for (let i = 0; i < border.length; i++) {
                let cell = border[i];
                if (this.field[cell.x][cell.y].type === 'O') {
                    canPlace = false;
                    break;
                }
            }

            if (canPlace) {
                for (let i = 0; i < border.length; i++) {
                    let cell = border[i];
                    this.field[cell.x][cell.y] = { type: ' ', x: cell.x, y: cell.y };
                }

                for (let i = 0; i < ship.length; i++) {
                    let cell = ship[i];
                    this.field[cell.x][cell.y] = { type: 'O', x: cell.x, y: cell.y, index: this.ships.length };
                }

                this.ships.push({
                    index: this.ships.length, ship, border, length
                });

                placed = true;

                break;
            }
        }

        if (!placed)
            console.warn("Ship was not placed", length);
    }

    genBotMoves(moves) {
        for (let i = 0; i < 10; i++) {
            for (let j = 0; j < 10; j++) {
                moves.push({ x: i, y: j });
            }
        }
    }

    shotFn({ x, y, cells }) {

        let cell = this.field[x][y];

        let shipIndex = cell.type === 'O' ? cell.index : -1;

        if (cell.type === 'O') {
            let shipLength = this.ships[shipIndex].length - 1

            this.ships[shipIndex].length = shipLength;
            this.ships[shipIndex].ship[shipLength].type = 'K';
        }

        cell.type = cell.type === 'O' ? 'K' : 'X';

        cells.push(cell)

        return cell.type === 'K';
    }

    updateAvailableMovesFn() {

        this.availableMoves = [];

        for (let i = 0; i < this.field.length; i++) {
            for (let j = 0; j < this.field[i].length; j++) {
                if (this.field[i][j].type === ' ' || this.field[i][j].type === 'O') {
                    this.availableMoves.push({ x: i, y: j });
                }
            }
        }
    }

    drawBorderFn({ cells }) {
        let clearPrevSuccessfullMoves = false;

        for (let i = 0; i < this.ships.length; i++) {
            let ship = this.ships[i];
            if (ship.length === 0 && !ship.processed) {
                for (let j = 0; j < ship.border.length; j++) {
                    let cell = ship.border[j];
                    this.field[cell.x][cell.y].type = 'B';
                    cells.push({ x: cell.x, y: cell.y, type: 'B' })
                    clearPrevSuccessfullMoves = true;
                    ship.processed = true;
                }
            }
        }

        if (clearPrevSuccessfullMoves) {
            this.prevSuccessfullMoves = [];
        }
    }

    generateNextMovesFn({ x, y }) {

        if (this.field[x][y].type === 'K') {

            this.nextPossibleMoves.push({ x: x - 1, y: y });

            this.nextPossibleMoves.push({ x: x, y: y + 1 });

            this.nextPossibleMoves.push({ x: x + 1, y: y });

            this.nextPossibleMoves.push({ x: x, y: y - 1 });

            for (let i = 0; i < this.nextPossibleMoves.length; i++) {
                let nextPossibleMove = this.nextPossibleMoves[i];
                if (this.availableMoves.findIndex(item => item.x === nextPossibleMove.x && item.y === nextPossibleMove.y) === -1) {
                    this.nextPossibleMoves.splice(i, 1);
                    i--;
                }
            }

            this.prevSuccessfullMoves.push({ x, y });

            if (this.prevSuccessfullMoves.length > 1) {
                let prevSuccessfullMove = this.prevSuccessfullMoves[this.prevSuccessfullMoves.length - 1];

                for (let index = this.prevSuccessfullMoves.length - 2; index >= 0; index--) {
                    let prevPrevSuccessfullMove = this.prevSuccessfullMoves[index];

                    if ((Math.abs(prevPrevSuccessfullMove.x - prevSuccessfullMove.x) === 0 && Math.abs(prevPrevSuccessfullMove.y - prevSuccessfullMove.y) === 1) ||
                        (Math.abs(prevPrevSuccessfullMove.x - prevSuccessfullMove.x) === 1 && Math.abs(prevPrevSuccessfullMove.y - prevSuccessfullMove.y) === 0)) {

                        if (prevPrevSuccessfullMove.x === prevSuccessfullMove.x) {
                            for (let i = 0; i < this.nextPossibleMoves.length; i++) {
                                if (this.nextPossibleMoves[i].x !== prevPrevSuccessfullMove.x) {
                                    this.nextPossibleMoves.splice(i, 1);
                                    i--;
                                }
                            }
                        } else {
                            for (let i = 0; i < this.nextPossibleMoves.length; i++) {
                                if (this.nextPossibleMoves[i].y !== prevPrevSuccessfullMove.y) {
                                    this.nextPossibleMoves.splice(i, 1);
                                    i--;
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    generateNextMoveFn() {
        let next = Math.floor(Math.random() * this.availableMoves.length);

        let move = this.availableMoves[next];

        if (this.nextPossibleMoves.length > 0) {
            move = this.nextPossibleMoves[Math.floor(Math.random() * this.nextPossibleMoves.length)];
        }

        return move
    }

    updateNextMovesFn({ x, y }) {
        let nextPossibleMoveIndex = this.nextPossibleMoves.findIndex(item => item.x === x && item.y === y);
        this.nextPossibleMoves.splice(nextPossibleMoveIndex, 1);
    }

    getAvailableShips() {
        let numOfShips = 0;
        for (let i = 0; i < this.ships.length; i++) {
            let ship = this.ships[i]
            if (ship.length > 0)
                numOfShips++;
        }
        return numOfShips
    }

    init() {
        this.availableMoves = [];
        this.nextPossibleMoves = [];
        this.prevSuccessfullMoves = [];
        this.field = [];
        this.ships = [];

        for (let i = 0; i < 10; i++) {
            this.field.push([]);
            for (let j = 0; j < 10; j++) {
                this.field[i].push({ type: ' ', x: i, y: j });
            }
        }

        for (let shipTypeIndex = 0; shipTypeIndex < this.shipsTypes.length; shipTypeIndex++) {
            this.placeShip({ length: this.shipsTypes[shipTypeIndex] });
        }

        this.genBotMoves(this.availableMoves);
    }
}