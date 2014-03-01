using UnityEngine;

using System;
using System.Linq;
using System.Collections.Generic;

public class InputFrame<T> : IEquatable<InputFrame<T> >
{
	public static InputFrame<T> Empty = new InputFrame<T>(default(T));

	public InputFrame(T keyPress)
	{
		Input = keyPress;
	}

	public T Input { get; set; }

	public bool Equals(InputFrame<T> rhs)
	{
		if (this.Input == null && rhs.Input == null)
		{
			return true;
		}

		return this.Input != null && this.Input.Equals(rhs.Input);
	}

	public static bool IsEmpty(InputFrame<T> rhs)
	{
		return rhs != null && Empty.Equals(rhs);
	}
}

/**
 * Interprets the input as a series of T objects 
 **/
public interface IInputInterpreter<T>
{
	/**
	 * Senses input and returns a series of respective T instances
	 * representing the input
	 **/
	InputFrame<T> Poll();
}

/**
 * Null implementation for an IInputInterpreter
 **/
public class NullInputInterpreter<T> : IInputInterpreter<T>
{
	/**
	 * Returns a null representation as the sensed input sequence
	 **/
	public InputFrame<T> Poll()
	{
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

	public InputFrame<T> Poll()
	{
		InputFrame<T> input = _impl.Poll();

		if (input != null)
		{
			if (InputFrame<T>.IsEmpty(input))
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
		}

		return input;
	}
}

public class KeyPress<T> : IEquatable<KeyPress<T> >
{
	public KeyPress() :
		this(default(T))
	{
	}
	
	public KeyPress(T key) :
		this(key, -1.0f)
	{
	}
	
	public KeyPress(T key, float minDuration) :
		this(key, minDuration, -1.0f)
	{
	}

	public KeyPress(T key, float minDuration, float maxDuration)
	{
		Key = key;
		MinDuration = minDuration;
		MaxDuration = maxDuration;
	}
	
	public T Key { get; set; }
	public float MinDuration { get; set; }
	public float MaxDuration { get; set; }
	
	public bool Equals(KeyPress<T> rhs)
	{
		return Key.Equals(rhs.Key) && MinDuration.Equals(rhs.MinDuration) && MaxDuration.Equals(rhs.MaxDuration);
	}
}

public interface ComboManager<T>
{
	void On(T[] keyCombination, Action<T[]> action);
	bool Off(T[] keyCombination, Action<T[]> action);

	T[] Poll();
}

/**
 * Reference: http://wiki.unity3d.com/index.php?title=KeyCombo
 **/
public class DefaultComboManager<T> : ComboManager<T>
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

	public DefaultComboManager() :
		this(5, new NullInputInterpreter<T>())
	{
	}

	public DefaultComboManager(int windowSize) :
		this(windowSize, new NullInputInterpreter<T>())
	{
	}

	public DefaultComboManager(int windowSize, IInputInterpreter<T> interpreter)
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
		InputFrame<T> input = _inputInterpreter.Poll();

		// If the inputs reference is null, represent it as
		// no key presses and not an empty input frame
		if (input != null)
		{
			_keyWindow.Add(input.Input);
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