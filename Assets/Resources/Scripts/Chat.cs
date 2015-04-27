using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Chat
{	
	List<ChatMessage> messages = new List<ChatMessage> ();

	public List<ChatMessage> GetMessages ()
	{
        List<ChatMessage> temp = new List<ChatMessage>(messages);
        //messages.Clear();
		return temp;
        
	}
    public List<ChatMessage> GetMessagesByTeam(int team)
    {
        List<ChatMessage> temp = new List<ChatMessage>();
        foreach (ChatMessage c in messages)
        {
            if (c.user.TeamNumber == team)
            {
                temp.Add(c);

            }
        }
        foreach (ChatMessage c in temp)
        {
            messages.Remove(c);
        }
        return temp;
    }
    public List<ChatMessage> GetMessagesByChannel(int channel)
    {
        List<ChatMessage> temp = new List<ChatMessage>();
        foreach (ChatMessage c in messages)
        {
            if (c.channel == channel)
            {
                temp.Add(c);

            }
        }
        foreach (ChatMessage c in temp)
        {
            messages.Remove(c);
        }
        return temp;
        

    }

	public void AddMessage (ChatMessage m)
	{
		messages.Add (m);
	}



	[Serializable]
	public class ChatMessage
	{
        public int channel;
		public string message;
		public User user;
		public ChatMessage(int c, string m, User u)
        {
			message = m;
            channel = c;
			user = u;
		}
	}
}
