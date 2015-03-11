using System.Collections;

/**
 * This class may be used to blindly create status checkables without any knowledge of the status checkables inner workings
 */
public class StatusCheckableFactory
{
	
	public StatusCheckableFactory ()
	{
	}
	
	/**
	 * creates and returns a status checkable built from the given protocol
	 */
	public StatusCheckable getStatusCheckableFromProtocol(StatusCheckableProtocol protocol) {
		if (protocol.HasAction) {
			ActionCheckable act = new ActionCheckable ();
			act.setFromProtocol (protocol);
			return act;
		} else if (protocol.HasLevel) {
			LevelCheckable level = new LevelCheckable ();
			level.setFromProtocol(protocol);
			return level;
		}

		MasterDriver.Instance.log ("Could not load Status Checkable from protobuf, status type may not be implemented yet.");
		return null;
	}
}