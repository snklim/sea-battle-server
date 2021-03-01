import './App.css';
import classNames from 'classnames';
import { useState, Fragment } from 'react';
import { useSelector, useDispatch, } from 'react-redux';
import { start1, move1, } from './store';

function Cell({ row, column, index }) {
  let field = useSelector(state => state.game.fields[index]);
  let nextPlayer = useSelector(state => state.game.nextPlayer);
  let player = useSelector(state => state.game.player);
  let name = useSelector(state => state.game.name);
  let prevMove = useSelector(state => state.game.prevMove[index]);
  let cell = field[row][column];

  let className = classNames('cell', 'cell-index-' + row + '-' + column, {
    'cell-ship': cell.type === 'O',
    'cell-border': cell.type === 'B',
    'cell-missed': cell.type === 'X',
    'cell-injured': cell.type === 'I',
    'cell-killed': cell.type === 'K',
    'cell-next-move': row === prevMove.x && column === prevMove.y && cell.type !== 'K',
  });

  let dispatch = useDispatch();

  return (
    <div className={className} onClick={() => {
      if (nextPlayer === player && index === 1) {
        dispatch(move1({ x: cell.x, y: cell.y, player: player, name: name }));
      }
    }}></div>
  );
}

function Row({ index, children }) {
  let className = classNames('row', 'row-index-' + index);

  return (
    <div className={className}>{children}</div>
  );
}

function Info() {
  let nextPlayer = useSelector(state => state.game.nextPlayer);
  let player = useSelector(state => state.game.player);
  let status = useSelector(state => state.game.status);

  return (
    <Fragment>
      {status === -1 && nextPlayer === player && <Row><span>Your turn</span></Row>}
      {status === -1 && nextPlayer !== player && <Row><span>Opponent turn</span></Row>}
      {status > -1 && status !== player && (<span>You lose</span>)}
      {status > -1 && status === player && (<span>You win</span>)}
    </Fragment>
  );
}

function Field({ index }) {
  let rows = [];

  let className = classNames('field', {
    'field-self': index === 0,
    'field-enemy': index === 1,
  });

  for (let i = 0; i < 10; i++) {
    let cells = [];
    for (let j = 0; j < 10; j++) {
      cells.push(<Cell key={i * 10 + j} row={i} column={j} index={index}></Cell>);
    }
    rows.push(<Row key={i} index={i}>{cells}</Row>);
  }

  return (
    <div className={className}>
      {rows}
    </div>
  );
}

function Game() {
  return (
    <Fragment>
      <Row>
        <Info></Info>
      </Row>
      <Field index={0}></Field>
      <Field index={1}></Field>
    </Fragment>
  );
}

function App() {
  let dispatch = useDispatch();
  let status = useSelector(state => state.game.status);

  let [name, setName] = useState('qwerty')
  let [playWithBot, setPlayWithBot] = useState(false)

  return (
    <div className='game'>
      {status >= -1 && (<Fragment><Game></Game><Row></Row></Fragment>)}
      {status !== -1 && (<Row>
        <input type='text' onChange={e => setName(e.target.value)} value={name}></input>
        <span>Play with bot:</span>
        <input type='checkbox' onChange={e => setPlayWithBot(e.target.checked)} checked={playWithBot}></input>
        <button onClick={() => dispatch(start1({ name, playWithBot }))}>Play</button>
      </Row>)}
    </div>
  );
}

export default App;
