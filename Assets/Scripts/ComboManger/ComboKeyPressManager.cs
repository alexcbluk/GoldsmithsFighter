using UnityEngine;

using System;
using System.Linq;
using System.Collections.Generic;

public class ComboKeyPressManager<T> : ComboManager<KeyPress<T> >
{
	private class KeyPressState<J>
	{
		public KeyPressState(KeyPress<J> keyPress)
		{
			KeyPress = keyPress;
			Duration = 0.0f;
		}

		public KeyPress<J> KeyPress { get; set; }
		public float Duration { get; set; }
	}

	// Aggregates the important attributes necessary to define and maintain a combo
	private class Combo<J>
	{
		public Combo(KeyPress<J>[] inputSequence)
		{
			InputSequence = inputSequence;
			Listeners = new List<Action<KeyPress<J>[]> >();

			InputSequenceState = new KeyPressState<J>[InputSequence.Length];
			for (int i = 0; i < InputSequence.Length; ++i)
			{
				InputSequenceState[i] = new KeyPressState<J>(InputSequence[i]);
			}

			Index = 0;
		}

		public KeyPress<J>[] InputSequence { get; set; }
		public List<Action<KeyPress<J>[]> > Listeners { get; set; }
		
		public KeyPressState<J>[] InputSequenceState { get; set; }
		public int Index { get; set; }

		public void reset()
		{
			foreach (KeyPressState<J> state in InputSequenceState)
			{
				state.Duration = 0.0f;
			}

			Index = 0;
		}

		public bool Equals(Combo<J> rhs)
		{
			return Enumerable.SequenceEqual(InputSequence, rhs.InputSequence);
		}
	}

	// The register containing all registered combos
	private List<Combo<T> > _comboRegister = null;

	// The input interpreter
	private IInputInterpreter<T> _interpreter = null;
	
	public ComboKeyPressManager()
		: this(new NullInputInterpreter<T>())
	{
	}

	public ComboKeyPressManager(IInputInterpreter<T> interpreter)
	{
		_comboRegister = new List<Combo<T> >();
		_interpreter = interpreter;
	}
	
	public IInputInterpreter<T> InputInterpreter
	{
		get
		{
			return _interpreter;
		}
		
		set
		{
			_interpreter = value;
		}
	}

	public void Register(KeyPress<T>[] keyCombination)
	{
		On(keyCombination, null);
	}

	private Combo<T> lookup(KeyPress<T>[] keyCombination)
	{
		Combo<T> rhs = new Combo<T>(keyCombination);
		foreach (Combo<T> combo in _comboRegister)
		{
			if (combo.Equals(rhs))
			{
				return combo;
			}
		}

		return null;
	}
	
	/**
	 *  Register an Action with the specified key combination 
	 **/
	public void On(KeyPress<T>[] keyCombination, Action<KeyPress<T>[]> action)
	{
		if (keyCombination != null)
		{
			Combo<T> combo = lookup(keyCombination);

			if (combo == null)
			{
				combo = new Combo<T>(keyCombination);
				_comboRegister.Add(combo);
			}

			if (action != null)
			{
				combo.Listeners.Add(action);
			}
		}
	}
	
	/**
	 *  Unregister a specific Action for the specified key combination 
	 **/
	public bool Off(KeyPress<T>[] keyCombination, Action<KeyPress<T>[]> action)
	{
		Combo<T> combo = lookup(keyCombination);
		return combo != null && combo.Listeners.Remove(action);
	}
	
	/**
	 *  Unregister all Actions for the specified key combination 
	 **/
	public bool Off(KeyPress<T>[] keyCombination)
	{
		return _comboRegister.Remove(new Combo<T>(keyCombination));
	}

	private Combo<T> MatchCombo(InputFrame<T> inputFrame)
	{
		Combo<T> success = null;

		for (int i = 0; i < _comboRegister.Count; ++i)
		{
			Combo<T> combo = _comboRegister[i];
			KeyPress<T> expectedKey = combo.InputSequence[combo.Index];

			//Debug.Log("Combo [" + i + "] - " + combo.Index);

			if (expectedKey.MinDuration < 0.0f)
			{
				if (expectedKey.Key.Equals(inputFrame.Input))
				{
					combo.Index++;
				}
				else
				{
					combo.reset();
				}
			}
			else
			{
				KeyPressState<T> expectedKeyState = combo.InputSequenceState[combo.Index];
				
				if (expectedKey.Key.Equals(inputFrame.Input))
				{
					expectedKeyState.Duration += Time.deltaTime;
					if (!(expectedKeyState.KeyPress.MaxDuration < 0.0f) && expectedKeyState.Duration > expectedKeyState.KeyPress.MaxDuration)
					{
						combo.reset();
					}
				}
				else if (expectedKeyState.Duration >= expectedKey.MinDuration)
				{
					//Debug.Log(expectedKeyState.KeyPress.Key.ToString() + " moving on to - " + (inputFrame.Input == null ? "null" : inputFrame.Input.ToString()));

					combo.Index++;
					if (combo.Index < combo.InputSequence.Length)
					{
						if (!combo.InputSequence[combo.Index].Key.Equals(inputFrame.Input))
						{
							//Debug.Log("Resetting Combo [" + i + "]");
							combo.reset();
						}
						else if (combo.InputSequence[combo.Index].MinDuration < 0.0f)
						{
							combo.Index++;
						}
					}
					//else
					//{
						//Debug.Log("Accepted");
					//}
				}
				else
				{
					combo.reset();
				}
			}
			
			// Successful combo
			if (combo.Index == combo.InputSequence.Length)
			{
				if (success == null || combo.InputSequence.Length >= success.InputSequence.Length)
				{
					success = combo;
				}

				combo.reset();
			}
		}

		return success;
	}

	/**
	 * Senses the input and calls the registered Action handlers
	 **/
	public KeyPress<T>[] Poll()
	{
		InputFrame<T> inputFrame = _interpreter.Poll();
		Combo<T> success = null;

		//Debug.Log(inputFrame == null ? "No Input" : (inputFrame.Input == null ? "Empty Frame" : inputFrame.Input.ToString()));

		// If the inputs reference is null, represent it as
		// no key presses and not an empty input frame
		if (inputFrame != null)
		{
			success = MatchCombo(inputFrame);

			if (success != null)
			{
				foreach(Action<KeyPress<T>[]> action in success.Listeners)
				{
					if (action != null)
					{
						action.Invoke(success.InputSequence);
					}
				}
			}
		}

		return (success == null ? null : success.InputSequence);
	}
}