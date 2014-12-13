using UnityEngine;
using System.Collections;

public abstract class ActionEventListener {

	/**
	 * The entire reason this is a virtual class instead of an interface
	 * is that this constructor masks the hookup of onAction to the
	 * ActionEventInvoker
	 */
	public ActionEventListener() {
		ActionEventInvoker.primaryInvoker.ActionEvent += new ActionEventHandler(onAction);
	}

	/**
	 * Action to perform on action event
	 */
	public abstract void onAction(Action action);
}
