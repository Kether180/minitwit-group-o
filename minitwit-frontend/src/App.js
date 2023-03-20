import { BrowserRouter as Router, Routes, Route } from "react-router-dom"

import logo from './logo.svg';
import './style/style.css';
import Registration from './components/Registration';
import Login from './components/Login.js';

function App() {
  return (
    <div className="App">
      <Router>
        <Routes>
          <Route exact path="/" element={<Login />} />
          <Route path="/Register" element={<Registration />} />
        </Routes>
      </Router>
 
    </div>
  );
}

export default App;
