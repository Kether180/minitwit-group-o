import React, {useState} from 'react';
import { useNavigate } from 'react-router-dom';
import '../style/login.css'

function Login() {

    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');
   
    let navigation = useNavigate();
    const goToRegister = () => {
        let path = '/Register';
        navigation(path);
    };

    const goToTimeline = () => {
        let path = '/Timeline';
        navigation(path);
    };

    const handleUsernameChange= (event) => {
        setUsername(event.target.value);
    };  
    
    const handlePasswordChange= (event) => {
        setPassword(event.target.value);
    } ;

    const handleSubmit = async (event) => {
        event.preventDefault();

        const response = await fetch("/api/login" , 
        {
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            },
            method: "POST",
            body: JSON.stringify({username: username, pwd: password })
        });

        if (response.ok) {
            goToTimeline();
        } else {
            // Login failed, display error message
            const errorData = await response.json();
            //setErrorMessage(errorData.message);
        }


    };

    return (
        <form onSubmit={handleSubmit}>
        <div className="container">        
            <div className='wrapper'>
                <div className="login-form">
                

                    <input type="username" placeholder='Username' value={username} onChange={handleUsernameChange} className="input-field form-item"/>
                    <input type="text" placeholder='E-mail' className="input-field form-item" />
                    <input type="password" placeholder='Password' value={password} onChange={handlePasswordChange} className="input-field form-item"/>
                    <button className="action-button form-item" type="submit">Login</button>
                    <p className='form-item text-field'>
                        If you do not have an account, then register here!
                    </p>
                    <button className="action-button form-item" onClick={goToRegister}>Register here</button>
                
                </div>
            </div>
        </div>
        </form>

    );

}

export default Login;

