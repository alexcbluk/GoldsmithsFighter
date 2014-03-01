public class P1InputInterpreter : DefaultInputInterpreter
{
	private static string[] BUTTONS = {
		"LightPunch",
		"HeavyPunch",
		"LightKick",
		"HeavyKick"
	};
	
	protected override string[] GetButtonIDs()
	{
		return BUTTONS;
	}
	
	protected override string GetHorizontalAxisLabel()
	{
		return "Horizontal";
	}
	
	protected override string GetVerticalAxisLabel()
	{
		return "Vertical";
	}
}