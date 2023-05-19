import React from 'react';
import { useNavigate } from 'react-router-dom';

import '../style/registration.css'

function Registration(){

    //On click of back button go to login
    let navigation = useNavigate();
    const goBack = () => {
        let path = '/';
        navigation(path);
    }

    //Register api call
    const register = () => {
        const username = document.querySelector('.js--username-register').value.trim();
        const email =  document.querySelector('.js--email-register').value.trim();
        const password =  document.querySelector('.js--password-register').value.trim();

        console.log(username, email, password);
        fetch("/api/register",
        {
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            },
            method: "POST",
            body: JSON.stringify({username: username, email: email, pwd: password })
        })
        .then(function(res) {console.log(res)})
        .catch(function(res) {console.log(res)});
    }
    

    return(
    <div className="container">        
        <div className='wrapper'>
            <div className="back-button-container">
                <button className="back-button action-button" onClick={goBack}>Back</button>
            </div>
            <div className="registration-form">

                <input type="text" placeholder='Username' className="input-field form-item js--username-register" />
                <input type="text" placeholder='E-mail' className="input-field form-item js--email-register" />
                <input type="password" placeholder='Password' className="input-field form-item js--password-register" />
                <button className="action-button form-item" onClick={register}>Register</button>
                
            </div>
        </div>
    </div>
    );
}

export default Registration;