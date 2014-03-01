public class P2InputInterpreter : DefaultInputInterpreter
{
	private static string[] BUTTONS = {
		"P2LightPunch",
		"P2HeavyPunch",
		"P2LightKick",
		"P2HeavyKick"
	};

	protected override string[] GetButtonIDs()
	{
		return BUTTONS;
	}
	
	protected override string GetHorizontalAxisLabel()
	{
		return "P2Horizontal";
	}
	
	protected override string GetVerticalAxisLabel()
	{
		return "P2Vertical";
	}
}