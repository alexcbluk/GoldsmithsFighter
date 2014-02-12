using UnityEngine;

using System;
using System.Collections.Generic;

/**
 * Interprets the input as a series of T objects 
 **/
public interface IInputInterpreter<T>
{
	/**
	 * Senses input and returns a series of respective T instances
	 * representing the input
	 **/
	T[] Poll();
}

/**
 * Null implementation for an IInputInterpreter
 **/
public class NullInputInterpreter<T> : IInputInterpreter<T>
{
	/**
	 * Returns a null representation as the sensed input sequence
	 **/
	public T[] Poll()
	{
		// Returns null for reference types or default values for value types
		//return GenerateEmptyFrame<T>();

		return null;
	}
}

/**
 * Decorator for IInputInterpreters which can allow input frames
 * to be skipped based on the delay value.
 **/ 
public class DelayInputInterpreter<T> : IInputInterpreter<T>
{
	private IInputInterpreter<T> _impl = null;
	private float _delay = 0.0f;
	private float _accumulator = 0.0f;

	public DelayInputInterpreter(IInputInterpreter<T> implementation) :
		this(implementation, 0.0f)
	{
	}
	
	public DelayInputInterpreter(IInputInterpreter<T> implementation, float delay)
	{
		Implementation = implementation;
		Delay = delay;
	}

	public float Delay {
		get { return _delay; }
		set
		{
			_delay = value;
			_accumulator = _delay;
		}
	}
	
	public IInputInterpreter<T> Implementation {
		get { return _impl; }
		set { _impl = (value == null ? new NullInputInterpreter<T>() : value); }
	}

	private bool IsEmptyFrame(T[] inputString)
	{
		return inputString != null && (inputString.Length == 1 && inputString[0] == null);
	}

	public T[] Poll()
	{
		T[] input = _impl.Poll();

		if (IsEmptyFrame(input))
		{
			if ((_accumulator -= Time.deltaTime) <= 0.0f)
			{
				_accumulator = _delay;
			
				// Empty input frame
				return input;
			}

			// Assume no key presses occured
			input = null;
		}
		else
		{
			// Reset timer
			_accumulator = _delay;
		}

		return input;
	}
}

public class KeyPress<T>
{
	KeyPress() :
		this(default(T))
	{
	}
		
	KeyPress(T key)
	{
		Key = key;
		Duration = 0.0f;
	}

	public float Duration { get; set; }
	public T Key { get; set; }
}

/**
 * Reference: http://wiki.unity3d.com/index.php?title=KeyCombo
 **/
public class ComboManager<T> 
{
	// The function type related for action events
	//public delegate void Action();

	// A map of input sequences to a series of registered actions
	private Dictionary<T[], List<Action<T[]> > > _keyComboRegister = null;

	// The list of recorded input
	private List<T> _keyWindow = null;
	//private List<T[]> _keyWindow = null;

	// The maximum input recordings
	private int _windowSize = 5;

	// The associated input interpreter
	private IInputInterpreter<T> _inputInterpreter = null;

	public ComboManager() :
		this(5, new NullInputInterpreter<T>())
	{
	}

	public ComboManager(int windowSize) :
		this(windowSize, new NullInputInterpreter<T>())
	{
	}

	public ComboManager(int windowSize, IInputInterpreter<T> interpreter)
	{
		_keyComboRegister = new Dictionary<T[], List<Action<T[]> > >();

		_windowSize = windowSize;
		_keyWindow = new List<T>(_windowSize);

		_inputInterpreter = interpreter;
	}

	public virtual IInputInterpreter<T> InputInterpreter
	{
		get
		{
			return _inputInterpreter;
		}

		set
		{
			_inputInterpreter = value;
		}
	}
	
	public void Register(T[] keyCombination)
	{
		On(keyCombination, null);
	}

	/**
	 *  Register an Action with the specified key combination 
	 **/
	public void On(T[] keyCombination, Action<T[]> action)
	{
		if (action != null)
		{
			if (!_keyComboRegister.ContainsKey(keyCombination))
			{
				_keyComboRegister[keyCombination] = new List<Action<T[]> >();
			}

			_keyComboRegister[keyCombination].Add(action);
		}
	}
	
	/**
	 *  Unregister a specific Action for the specified key combination 
	 **/
	public bool Off(T[] keyCombination, Action<T[]> action)
	{
		return _keyComboRegister.ContainsKey(keyCombination) &&
			_keyComboRegister[keyCombination].Remove(action);
	}
	
	/**
	 *  Unregister all Actions for the specified key combination 
	 **/
	public bool Off(T[] keyCombination)
	{
		return _keyComboRegister.ContainsKey(keyCombination) &&
			_keyComboRegister.Remove(keyCombination);
	}
	
	private static string ToString(T[] array)
	{
		return ToString(array, ",");
	}

	private static string ToString(T[] array, string delimiter)
	{
		string rvalue = "[";

		for (int i = 0; i < array.Length; ++i)
		{
			rvalue += (array[i] == null ? "null" : array[i].ToString());

			if (i < array.Length - 1)
			{
				rvalue += delimiter;
			}
		}

		return rvalue + "]";
	}

	/**
	 *  Returns true if the input key combinations have been pressed in succession
	 **/
	private bool IsInputted(T[] inputString) {
		// Find the starting index where to start looking through the recorded inputs
		int index = _keyWindow.Count - inputString.Length;
		int matchLength = 0;

		if (index >= 0)
		{
			// Check all of the input sequences
			for (int i = 0; index < _keyWindow.Count; ++index, ++i)
			{
				if ((_keyWindow[index] == null && inputString[i] == null) ||
				    (_keyWindow[index] != null && _keyWindow[index].Equals(inputString[i])))
				{
					++matchLength;
				}
			}
		}

		// Return true, if the input sequence matches the previous inputs
		return matchLength == inputString.Length;
	}

	/**
	 * Call all registered Action handlers which match the recent input pattern
	 **/
	private T[] CheckCombos() {
		T[] combo = null;

		foreach(KeyValuePair<T[], List<Action<T[]> > > entry in _keyComboRegister)
		{
			if (IsInputted(entry.Key))
			{
				if (combo == null || combo.Length < entry.Key.Length)
				{
					// Keep a record of the longest combo string match
					combo = entry.Key;
				}
			}
		}

		return combo;
	}
	
	/**
	 * Senses the input and calls the registered Action handlers
	 **/
	public T[] Poll()
	{
		T[] inputs = _inputInterpreter.Poll();

		// If the inputs reference is null, represent it as
		// no key presses and not an empty input frame
		if (inputs != null)
		{
			foreach (T input in inputs)
			{
				_keyWindow.Add(input);
			}
		}

		while (_keyWindow.Count > _windowSize) 
		{
			_keyWindow.RemoveAt(0);
		}

		Debug.Log(ToString(_keyWindow.ToArray()));

		T[] combo = CheckCombos();

		// Notify observers
		if (combo != null && _keyComboRegister.ContainsKey(combo))
		{
			foreach(Action<T[]> action in _keyComboRegister[combo])
			{
				if (action != null)
				{
					action.Invoke(combo);
				}
			}
		}

		return combo;
	}

}

/**
 *  Default implementation for an IInputInterpreter using Input.GetButton()/Input.GetAxis()
 **/
public class DefaultInputInterpreter : IInputInterpreter<string>
{
	protected static string[] EMPTY_FRAME = Encapsulate(null);
	
	private static string[] BUTTONS = {
		"Fire1",
		"Fire2",
		"Fire3",
		"Jump"
	};
	
	private float horizontal = 0.0f;
	private float vertical = 0.0f;
	
	protected virtual string[] GetButtonIDs()
	{
		return BUTTONS;
	}
	
	private static string[] Encapsulate(string str)
	{
		return new string[] { str };
	}
	
	public virtual string[] Poll()
	{
		float axis = Input.GetAxisRaw("Horizontal");
		if (axis != horizontal)
		{
			horizontal = axis;
			
			if (horizontal >= 1.0f)
			{
				return Encapsulate("Right");
			}
			else if (horizontal <= -1.0f)
			{
				return Encapsulate("Left");
			}
		}
		
		axis = Input.GetAxisRaw("Vertical");
		if (axis != vertical)
		{
			vertical = axis;

			if (vertical >= 1.0f)
			{
				return Encapsulate("Up");
			}
			else if (vertical <= -1.0f)
			{
				return Encapsulate("Down");
			}
		}
		
		foreach (string button in GetButtonIDs())
		{
			if (Input.GetButtonDown(button))
			{
				return Encapsulate(button);
			}
		}
		
		// Empty input frame
		return EMPTY_FRAME;
	}
}

/**
 *  Default ComboManger for string types 
 **/
public class KeyComboManager : ComboManager<string>
{
	public KeyComboManager() :
		this(0.2f)
	{
	}
	
	public KeyComboManager(float keyDelay) :
		this(keyDelay, 5)
	{
	}

	public KeyComboManager(float keyDelay, int windowSize) :
		this(keyDelay, windowSize, new DefaultInputInterpreter())
	{
	}
	
	public KeyComboManager(float keyDelay, int windowSize, IInputInterpreter<string> interpreter) :
		// Always encapsulate the interpreter within a DelayInputInterpreter wrapper
		base(windowSize, new DelayInputInterpreter<string>(interpreter))
	{
		Delay = keyDelay;
	}

	public override IInputInterpreter<string> InputInterpreter
	{
		get
		{
			return ((DelayInputInterpreter<string>) base.InputInterpreter).Implementation;
		}

		set
		{
			((DelayInputInterpreter<string>) base.InputInterpreter).Implementation = value;
		}
	}

	public float Delay
	{
		get
		{
			return ((DelayInputInterpreter<string>) base.InputInterpreter).Delay;
		}

		set
		{
			((DelayInputInterpreter<string>) base.InputInterpreter).Delay = value;
		}
	}
}