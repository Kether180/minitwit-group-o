import React from 'react';
import { useEffect, useState } from 'react';
import '../style/timeline.css'

function Timeline(){

    const [msgs, setMsgs] = useState([]);
    const [followsMap, setFollowsMap] = useState(new Map());
    const updateFollowsMap = (k,v) => {
        setFollowsMap(new Map(followsMap.set(k,v)));
    }
    
    let loggedUser = localStorage.getItem('username');
    const url = `/api/fllws/${loggedUser}`
    

    useEffect(() => {

      const fetchMsgs = async (f) => {
        fetch("/api/msgs", {    
            method: 'GET'     
          }).then((response) => {
            console.log(response); 
            return response.json();
          })
            .then((data) => {  
              const finalMsgs = data.map((msg) => {
                const copy = {...msg};
                const follow = f.find(follower => follower.name === copy.user);
                
                if(follow != null) {
                    console.log(follow);
                    copy.follow = follow.name;
                }
                return copy 

              });
                
              setMsgs(finalMsgs);
              
            })
            .catch((error) => console.log(error));
          }
       
          const fetchFollows = async () => {
            try {
              const response = await fetch(url, { method: 'GET' });
              const data = await response.json();
              const follows = data.follows.map(name => ({ name }));
              const followers = follows.map((user) => {
                updateFollowsMap(user.name, true);
              })
              return follows;
            } catch (error) {
              console.log(error);
              return [];
            }
          }

      fetchFollows().then((f) => fetchMsgs(f));

        
      }, []);

    const handleFollow = async (user) => {
      let body = {follow: user, unfollow: null };

      if (followsMap.get(user)) {
        body = {follow: null, unfollow: user };
      }
      
      var url = `/api/fllws/${loggedUser}`
      const response = await fetch(url , 
      {
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            },
            method: "POST",
            body: JSON.stringify(body)
        });

        if (response.ok) {
            updateFollowsMap(user, !followsMap.get(user))
        } else {
            const errorData = await response.json();
        }

    }

    

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
                <button className="follow-button" onClick={() => handleFollow(item.user)}>
                  { followsMap.get(item.user) ? 'Unfollow' : 'Follow'}
                </button>
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