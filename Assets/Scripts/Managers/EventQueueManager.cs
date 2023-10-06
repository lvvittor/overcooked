using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventQueueManager : MonoBehaviour {
    public static EventQueueManager Instance { get; private set; }

    private void Awake() {
        Instance = this;
    }

    public Queue<ICommand> EventQueue => _eventQueue;
    private Queue<ICommand> _eventQueue = new Queue<ICommand>();

    public void Update() {
        while (_eventQueue.Count > 0) {
            ICommand command = _eventQueue.Dequeue();
            command.Do();
        }
    }

    public void AddCommand(ICommand command) => _eventQueue.Enqueue(command);
}
