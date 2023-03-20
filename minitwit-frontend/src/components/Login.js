import React, {useState} from 'react';
import { useNavigate } from 'react-router-dom';
import '../style/login.css'

function Login() {
   
    let navigation = useNavigate();
    const goToRegister = () => {
        let path = '/Register';
        navigation(path);
    }
    
    return (
        <div className="container">        
            <div className='wrapper'>
                <div className="login-form">

                    <input type="text" placeholder='Username' className="input-field form-item" />
                    <input type="text" placeholder='E-mail' className="input-field form-item" />
                    <input type="password" placeholder='Password' className="input-field form-item" />
                    <button className="action-button form-item">Login</button>
                    <p className='form-item text-field'>
                        If you do not have an account, then register here!
                    </p>
                    <button className="action-button form-item" onClick={goToRegister}>Register here</button>
                </div>
            </div>
        </div>

    );

}

export default Login;

