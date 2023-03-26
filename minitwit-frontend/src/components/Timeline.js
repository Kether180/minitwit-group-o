import React from 'react';
import { useEffect, useState } from 'react';

function Timeline(){

    const [msgs, setMsgs] = useState([]);

    useEffect(() => {
        fetch("http://157.230.79.99:5050/msgs", {    
            method: 'GET',      
            mode: 'no-cors'     
          }).then((response) => response.json())
            .then((data) => {    
              console.log(data);    
              setMsgs(data);
            })
      }, []);

    return (
        <div>
            {msgs.map((item) => (
                <p>{item.content}</p>
            ))}
        </div>
    );
}

export default Timeline;