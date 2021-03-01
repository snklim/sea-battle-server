import { createSlice, configureStore, createAsyncThunk } from '@reduxjs/toolkit';
import { withCallbacks, signalMiddleware, LogLevel } from 'redux-signalr'
import { HubConnectionBuilder, HttpTransportType } from '@microsoft/signalr';

const initialState = {
    fields: [[], []],
    nextPlayer: 1,
    player: -1,
    nextMove: [null, null],
    prevMove: [{ x: -1, y: -1 }, { x: -1, y: -1 }],
    availableShips: [10, 10],
    status: -2,
    name: ''
}

const connection = new HubConnectionBuilder()
    .configureLogging(LogLevel.Debug)
    .withUrl('http://localhost:5000/hubs/chat', {
        skipNegotiation: true,
        transport: HttpTransportType.WebSockets,
    })
    .build();

export const start = createAsyncThunk('game/start', async () => {
    let response = await window.fetch("/api/game")

    let data = await response.json()

    return data
})

export const move = createAsyncThunk('game/move', async ({ x, y, fieldIndex }) => {

    let response = await window.fetch("/api/game", { method: 'POST', body: JSON.stringify({ x, y, fieldIndex }) })

    let data = await response.json()

    return data
})

export const sendMessage = (txt) => (dispatch, getState, invoke) => {
    invoke('SendMessage', txt)
};

export const start1 = (request) => (dispatch, getState, invoke) => {
    invoke('Start', request)
};

export const move1 = (move) => (dispatch, getState, invoke) => {
    invoke('Move', move)
};

function onStarted(state, action) {
    let { nextPlayer, player, nextMove, fields, name } = action.payload;

    state.fields = [[], []];
    state.nextPlayer = 1;
    state.nextMove = [null, null];
    state.prevMove = [{ x: -1, y: -1 }, { x: -1, y: -1 }];
    state.availableShips = [10, 10];

    state.status = -1;

    state.nextPlayer = nextPlayer;
    state.player = player;
    state.name = name;

    state.nextMove[0] = nextMove[0];
    state.nextMove[1] = nextMove[1];

    for (let fieldIndex = 0; fieldIndex < 2; fieldIndex++) {
        for (let i = 0; i < 10; i++) {
            state.fields[fieldIndex].push([])

            for (let j = 0; j < 10; j++) {
                let { x, y, type } = fields[fieldIndex][i][j];
                state.fields[fieldIndex][i].push({ x, y, type });
            }
        }
    }
}

function onMoved(state, action) {
    let { valid, x, y, cells, fieldIndex, nextPlayer, nextMove, status } = action.payload

    if (valid) {
        for (let i = 0; i < cells.length; i++) {
            let cell = cells[i]
            state.fields[fieldIndex][cell.x][cell.y].type = cell.type
        }

        state.nextPlayer = nextPlayer
        state.nextMove[fieldIndex] = nextMove
        state.prevMove[fieldIndex] = { x, y }
        state.status = status
    }
}

const gameSlice = createSlice({
    name: 'game',
    initialState,
    reducers: {
        started: (state, action) => {
            onStarted(state, action)
        },
        moved: (state, action) => {
            onMoved(state, action)
        }
    },
    extraReducers: {
        [start.fulfilled]: (state, action) => {
            onStarted(state, action)
        },
        [move.fulfilled]: (state, action) => {
            onMoved(state, action)
        }
    }
})

let { started, moved } = gameSlice.actions

const callbacks = withCallbacks()
    .add('Started', (data) => (dispatch) => {
        dispatch(started(data));
    })
    .add('Moved', (data, fieldIndex) => (dispatch) => {
        dispatch(moved({ ...data, fieldIndex }));
    })

const signal = signalMiddleware({
    callbacks,
    connection,
});

export default configureStore({
    reducer: {
        game: gameSlice.reducer
    },
    middleware: [signal],
})