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
      <div className= 'container'>
        <div className='centered'>
          
          <h1>Timeline</h1>
          {msgs.map((item) => (
            <div key={item.pub_date}>
              <p>{item.user}</p>
              <p>{item.content}</p>
            </div>
          ))}
        </div>
      </div>
      </>
    );
}

export default Timeline;