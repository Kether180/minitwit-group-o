import React from 'react';
import '../style/registration.css'

function Registration(){
    return(
    <div className="container">        
        <div className='wrapper'>
            <div className="registration-form">

                <input type="text" placeholder='Username' className="input-field form-item" />
                <input type="text" placeholder='E-mail' className="input-field form-item" />
                <input type="password" placeholder='Password' className="input-field form-item" />
                <button className="action-button form-item">Register</button>
                
            </div>
        </div>
    </div>
    );
}

export default Registration;