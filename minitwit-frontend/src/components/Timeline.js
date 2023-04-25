import React from 'react';
import { useEffect, useState } from 'react';
import '../style/timeline.css'

function Timeline(){

    const [msgs, setMsgs] = useState([]);

    useEffect(() => {
        fetch("/api/msgs", {    
            method: 'GET'     
          }).then((response) => {
            console.log(response); 
            return response.json();
          })
            .then((data) => {    
              console.log(data);    
              setMsgs(data);
            })
            .catch((error) => console.log(error));
      }, []);

    return (
      <>
      <br/>

      <br/>      
      <div className="container">
      <div className="centered">
        <h1 className="timeline-header">Timeline</h1>
        {msgs.map((item) => (
          <div key={item.pub_date} className="post-container">
            <div className="post-header">
              <p className="post-user">{item.user}</p>
              <div className="post-buttons">
                <button className="follow-button">Follow</button>
              </div>
            </div>
            <div className="post-content">
              <div className="post-box">
                <p className="post-text">{item.content}</p>
              </div>
              <br/>
            </div>
          </div>
        ))}
      </div>
    </div>
      </>
    );
}

export default Timeline;