using UnityEngine;
using System.Collections;

public abstract class ActionEventListener {

	/**
	 * Register this listener with the primary action event invoker
	 */
	public void register() {
		ActionEventInvoker.primaryInvoker.ActionEvent += onAction;
	}

	/**
	 * Deregister this listener with the primary action event invoker
	 */
	public void deregister() {
		ActionEventInvoker.primaryInvoker.ActionEvent -= onAction;
	}

	/**
	 * Action to perform on action event
	 */
	public abstract void onAction(IAction action);
}
