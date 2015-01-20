using UnityEngine;
using System.Collections;

public delegate void ActionEventHandler(IAction action);

public class ActionEventInvoker {

	private static ActionEventInvoker instance;

	public static ActionEventInvoker primaryInvoker {
		get {
			if(instance == null) {
				instance = new ActionEventInvoker();
				//first thing registered should always be the status, so status SHOULD NOT call its base constructor
				instance.ActionEvent += new ActionEventHandler(Status.playerStatus.onAction);
			}
			return instance;
		}
	}

	private ActionEventInvoker() {

	}

	/**
	 * The event identifier
	 */
	public event ActionEventHandler ActionEvent;

	/**
	 * Call this to invoke this action across all objects listening
	 * to the invoker
	 */
	public void invokeAction(IAction action) {
		ActionEvent (action);
	}
}
