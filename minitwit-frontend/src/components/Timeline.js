import React from 'react';
import { useEffect, useState } from 'react';

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
        <div>
            {msgs.map((item) => (
                <p>{item.content}</p>
            ))}
        </div>
    );
}

export default Timeline;