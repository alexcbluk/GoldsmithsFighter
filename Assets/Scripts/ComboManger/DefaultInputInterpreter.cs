using UnityEngine;

using System;

/**
 *  Default implementation for an IInputInterpreter using Input.GetButton()/Input.GetAxis()
 **/
public class DefaultInputInterpreter : IInputInterpreter<string>
{
	private static string[] BUTTONS = {
		"Fire1",
		"Fire2",
		"Fire3",
		"Jump"
	};

	protected virtual string[] GetButtonIDs()
	{
		return BUTTONS;
	}

	protected virtual string GetHorizontalAxisLabel()
	{
		return "Horizontal";
	}
	
	protected virtual string GetVerticalAxisLabel()
	{
		return "Vertical";
	}
	
	private static InputFrame<string> Encapsulate(string str)
	{
		return new InputFrame<string>(str);
	}
	
	public virtual InputFrame<string> Poll()
	{
		foreach (string button in GetButtonIDs())
		{
			if (Input.GetButtonDown(button))
			{
				return Encapsulate(button);
			}
		}
		
		float axis = Input.GetAxisRaw(GetHorizontalAxisLabel());
		if (axis >= 1.0f)
		{
			return Encapsulate("Right");
		}
		else if (axis <= -1.0f)
		{
			return Encapsulate("Left");
		}
		
		axis = Input.GetAxisRaw(GetVerticalAxisLabel());
		if (axis >= 1.0f)
		{
			return Encapsulate("Up");
		}
		else if (axis <= -1.0f)
		{
			return Encapsulate("Down");
		}

		// Empty input frame
		return InputFrame<string>.Empty;
	}
}